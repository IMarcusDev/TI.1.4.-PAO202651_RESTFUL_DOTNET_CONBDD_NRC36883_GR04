using ec.edu.monster.config;
using ec.edu.monster.fakes;
using ec.edu.monster.modelos.dto;
using ec.edu.monster.modelos.rest;
using Microsoft.Extensions.Options;

namespace ec.edu.monster.modelos.servicios;

public class LoginServiceTest
{
    private readonly FakeTransporte _transporte = new();
    private readonly LoginService _servicio;

    public LoginServiceTest()
        => _servicio = new LoginService(_transporte, Options.Create(new ServidorOptions { Host = "10.0.0.5", Puerto = 5088 }));

    [Fact]
    public async Task LoginEmpleadoSendsUsuarioAndClaveToTheLoginServiceAndReadsTheUser()
    {
        _transporte.Responder("loginEmpleado", RestRespuestas.LoginOk("Monster Bank", "Empleado"));

        LoginResultado resultado = await _servicio.LoginEmpleadoAsync("monster", "monster9");

        PeticionRest peticion = _transporte.Ultima("loginEmpleado");
        Assert.Equal("http://10.0.0.5:5088/Login/loginEmpleado", peticion.Url);
        Assert.Equal("monster", peticion.Parametro("usuario"));
        Assert.Equal("monster9", peticion.Parametro("clave"));
        Assert.Equal(new LoginResultado(true, "Ingreso de sesión exitoso", "token-123", "Monster Bank", "Empleado"), resultado);
    }

    [Fact]
    public async Task LoginClienteSendsTheAccountNumber()
    {
        _transporte.Responder("loginCliente", RestRespuestas.LoginOk("Ana Pérez", "Cliente"));

        LoginResultado resultado = await _servicio.LoginClienteAsync("00100001", "123456");

        Assert.Equal("00100001", _transporte.Ultima("loginCliente").Parametro("cuenta"));
        Assert.Equal("Cliente", resultado.TipoUsuario);
    }

    [Fact]
    public async Task ReturnsTheServerMessageOnError()
    {
        _transporte.Responder("loginEmpleado", RestRespuestas.Error("Usuario o clave incorrectos"));

        Assert.Equal(LoginResultado.Error("Usuario o clave incorrectos"), await _servicio.LoginEmpleadoAsync("x", "y"));
    }

    [Fact]
    public async Task ReportsTheServerAddressWhenItCannotConnect()
    {
        LoginResultado resultado = await _servicio.LoginEmpleadoAsync("x", "y");

        Assert.Equal(LoginResultado.Error("No se pudo conectar con el servidor: http://10.0.0.5:5088/Login"), resultado);
    }

    [Fact]
    public async Task ShowsTheHttpCodeWhenTheServerAnswersAnErrorWithoutEstado()
    {
        _transporte.Responder("loginEmpleado", 404, "");

        Assert.Equal(LoginResultado.Error(ServicioRest.FallaServidor(404)), await _servicio.LoginEmpleadoAsync("x", "y"));
    }

    [Fact]
    public async Task ShowsTheServerMessageOfAnHttpErrorThatCarriesEstado()
    {
        _transporte.Responder("loginEmpleado", 401, RestRespuestas.Error("Usuario inactivo"));

        Assert.Equal(LoginResultado.Error("Usuario inactivo"), await _servicio.LoginEmpleadoAsync("x", "y"));
    }

    [Fact]
    public async Task TreatsAnOkWithoutTokenAsAnIncompleteResponse()
    {
        _transporte.Responder("loginEmpleado", "{\"estado\":0}");

        Assert.Equal(LoginResultado.Error(ServicioRest.RespuestaIncompleta), await _servicio.LoginEmpleadoAsync("x", "y"));
    }

    [Fact]
    public async Task LogoutSendsTheToken()
    {
        _transporte.Responder("logout", RestRespuestas.Exito("Sesión cerrada"));

        Assert.True(await _servicio.LogoutAsync("token-123"));
        Assert.Equal("token-123", _transporte.Ultima("logout").Parametro("token"));
    }
}

public class MovimientoServiceTest
{
    private readonly FakeTransporte _transporte = new();
    private readonly MovimientoService _servicio;

    public MovimientoServiceTest()
        => _servicio = new MovimientoService(_transporte, Options.Create(new ServidorOptions()));

    [Fact]
    public async Task RegistrarDepositoSendsTokenAccountAndAmountWithoutKey()
    {
        _transporte.Responder("registrarDeposito", RestRespuestas.MovimientoOk("Depósito registrado exitosamente", 7, "620.00"));

        MovimientoResultado resultado = await _servicio.RegistrarDepositoAsync("t", "00100001", 120.5m);

        PeticionRest peticion = _transporte.Ultima("registrarDeposito");
        Assert.Equal("http://localhost:5088/Movimiento/registrarDeposito", peticion.Url);
        Assert.Equal(("t", "00100001", "120.50", (string?)null),
            (peticion.Parametro("token"), peticion.Parametro("cuenta"), peticion.Parametro("importe"), peticion.Parametro("clave")));
        Assert.Equal(System.Text.Json.JsonValueKind.Number, peticion.Tipo("importe"));
        Assert.Equal(new MovimientoResultado(true, "Depósito registrado exitosamente", "7", 620.00m), resultado);
    }

    [Fact]
    public async Task RegistrarRetiroSendsTheKeyAsTyped()
    {
        _transporte.Responder("registrarRetiro", RestRespuestas.MovimientoOk("ok", 8, "500.00"));

        await _servicio.RegistrarRetiroAsync("t", "00100001", 10m, " clave ");

        Assert.Equal(" clave ", _transporte.Ultima("registrarRetiro").Parametro("clave"));
    }

    [Fact]
    public async Task RegistrarTransferenciaSendsBothAccounts()
    {
        _transporte.Responder("registrarTransferencia", RestRespuestas.MovimientoOk("ok", 9, "400.00"));

        await _servicio.RegistrarTransferenciaAsync("t", "00100001", "00200002", 99.99m, "");

        PeticionRest peticion = _transporte.Ultima("registrarTransferencia");
        Assert.Equal(("00100001", "00200002", "99.99", ""),
            (peticion.Parametro("cuentaOrigen"), peticion.Parametro("cuentaDestino"), peticion.Parametro("importe"), peticion.Parametro("clave")));
    }

    [Fact]
    public async Task AnInvalidTokenMarksTheSessionAsExpired()
    {
        _transporte.Responder("registrarRetiro", RestRespuestas.Error("Token inválido o sesión expirada"));

        MovimientoResultado resultado = await _servicio.RegistrarRetiroAsync("t", "1", 1m, "k");

        Assert.Equal(MovimientoResultado.Error("Token inválido o sesión expirada", sesionExpirada: true), resultado);
    }

    [Fact]
    public async Task ABusinessErrorKeepsTheServerMessage()
    {
        _transporte.Responder("registrarRetiro", RestRespuestas.Error("Saldo insuficiente"));

        Assert.Equal(MovimientoResultado.Error("Saldo insuficiente"), await _servicio.RegistrarRetiroAsync("t", "1", 1m, "k"));
    }

    [Fact]
    public async Task ConsultarMovimientosReadsTheAccountAndFormatsTheRowsNewestFirst()
    {
        _transporte.Responder("consultarMovimientos", RestRespuestas.ConsultaOk(
            (1, "2026-10-30T09:15:00", "Depósito", "INGRESO", "500.00", ""),
            (6, "2026-10-31T16:40:00", "Transferencia", "SALIDA", "120.50", "00200002")));

        HistorialCuenta historial = await _servicio.ConsultarMovimientosAsync("t", "00100001");

        Assert.Equal("00100001", _transporte.Ultima("consultarMovimientos").Parametro("cuenta"));
        Assert.True(historial.Ok);
        Assert.Equal(new CuentaConsultada("00100001", "Ana Pérez", "USD", 379.50m, "ACTIVO"), historial.Cuenta);
        Assert.Equal(
            [
                new MovimientoConsultado("000006", "31/10/2026 16:40", "Transferencia", "SALIDA", 120.50m, "00200002"),
                new MovimientoConsultado("000001", "30/10/2026 09:15", "Depósito", "INGRESO", 500.00m, "-"),
            ],
            historial.Movimientos);
    }

    [Fact]
    public async Task ConsultarMovimientosWithAnInvalidTokenIsAnExpiredSession()
    {
        _transporte.Responder("consultarMovimientos", RestRespuestas.Error("Token inválido o sesión expirada"));

        HistorialCuenta historial = await _servicio.ConsultarMovimientosAsync("t", "1");

        Assert.False(historial.Ok);
        Assert.True(historial.SesionExpirada);
    }
}
