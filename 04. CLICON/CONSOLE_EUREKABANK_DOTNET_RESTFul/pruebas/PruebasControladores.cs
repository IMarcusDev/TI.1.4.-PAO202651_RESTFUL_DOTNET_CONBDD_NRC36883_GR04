//revisado y aprovado por Marcos Escobar
using System.Net;
using System.Text;
using System.Text.Json;
using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.Enums;
using CONSOLE_EUREKABANK_DOTNET_RESTFul.Controllers;
using CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.HTTP;

namespace CONSOLE_EUREKABANK_DOTNET_RESTFul.Pruebas;

/// <summary>Reemplaza la red: guarda la última petición y devuelve la respuesta preparada.</summary>
internal sealed class ServidorFalso : HttpMessageHandler
{
    public string Url = "";
    public string Cuerpo = "";
    public string Respuesta = """{"estado":0,"mensaje":"ok"}""";
    public HttpStatusCode Codigo = HttpStatusCode.OK;
    public bool SinRed;

    public JsonElement Json => JsonDocument.Parse(Cuerpo).RootElement;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token)
    {
        Url = request.RequestUri!.ToString();
        Cuerpo = request.Content is null ? "" : await request.Content.ReadAsStringAsync(token);
        if (SinRed)
            throw new HttpRequestException("sin red");
        return new HttpResponseMessage(Codigo) { Content = new StringContent(Respuesta, Encoding.UTF8, "application/json") };
    }
}

internal static class PruebasControladores
{
    private const string Base = "http://servidor:5088";

    private static ServidorFalso Preparar(string respuesta = """{"estado":0,"mensaje":"ok"}""")
    {
        var falso = new ServidorFalso { Respuesta = respuesta };
        ConfigHTTP.Handler = falso;
        ConfigHTTP.BaseUrl = Base;
        ConfigHTTP.Limpiar();
        return falso;
    }

    private static void SesionEmpleado()
    {
        ConfigHTTP.Token = "tok-e";
        ConfigHTTP.Perfil = ConfigHTTP.EMPLEADO;
    }

    public static Ejecutor Crear()
    {
        var e = new Ejecutor();

        e.Caso("loginEmpleado: ruta, cuerpo y sesión del empleado", async () =>
        {
            var s = Preparar("""{"estado":0,"mensaje":"Ingreso de sesión exitoso","token":"tok-e","perfil":"EMPLEADO","nombre":"Carla Andrade"}""");
            var r = await new LoginController().LoginEmpleadoAsync("cajero", "cajero123");
            Afirmar.Igual(EstadosEnum.OK, r.GetEstado());
            Afirmar.Igual(Base + "/Login/loginEmpleado", s.Url);
            Afirmar.Igual("cajero", s.Json.GetProperty("usuario").GetString());
            Afirmar.Igual("cajero123", s.Json.GetProperty("clave").GetString());
            Afirmar.Igual("tok-e", ConfigHTTP.Token);
            Afirmar.Falso(ConfigHTTP.EsCliente(), "perfil empleado");
            Afirmar.Igual("", ConfigHTTP.Cuenta);
        });

        e.Caso("loginCliente guarda la cuenta de la sesión", async () =>
        {
            var s = Preparar("""{"estado":0,"mensaje":"ok","token":"tok-c","perfil":"CLIENTE","nombre":"Ana Torres"}""");
            await new LoginController().LoginClienteAsync("00100001", "123456");
            Afirmar.Igual(Base + "/Login/loginCliente", s.Url);
            Afirmar.Igual("00100001", s.Json.GetProperty("cuenta").GetString());
            Afirmar.Verdadero(ConfigHTTP.EsCliente(), "perfil cliente");
            Afirmar.Igual("00100001", ConfigHTTP.Cuenta);
        });

        e.Caso("Login rechazado no abre sesión", async () =>
        {
            Preparar("""{"estado":1,"mensaje":"Credenciales incorrectas","token":""}""");
            var r = await new LoginController().LoginClienteAsync("00100001", "999999");
            Afirmar.Igual("Credenciales incorrectas", r.GetMensaje());
            Afirmar.Falso(ConfigHTTP.HaySesion(), "sin sesión");
        });

        e.Caso("Sin servidor o respuesta que no es JSON", async () =>
        {
            var s = Preparar();
            s.SinRed = true;
            var r = await new LoginController().LoginEmpleadoAsync("cajero", "x");
            Afirmar.Igual(ErroresCliente.SinServidor, r.GetMensaje());
            s.SinRed = false;
            s.Codigo = HttpStatusCode.NotFound;
            s.Respuesta = "<html>404</html>";
            r = await new LoginController().LoginEmpleadoAsync("cajero", "x");
            Afirmar.Igual(ErroresCliente.RespuestaIncompleta, r.GetMensaje());
        });

        e.Caso("logout envía el token y borra la sesión aunque falle la red", async () =>
        {
            var s = Preparar();
            SesionEmpleado();
            s.SinRed = true;
            await new LoginController().LogoutAsync();
            Afirmar.Igual(Base + "/Login/logout", s.Url);
            Afirmar.Igual("tok-e", s.Json.GetProperty("token").GetString());
            Afirmar.Falso(ConfigHTTP.HaySesion(), "sesión borrada");
        });

        e.Caso("consultarMovimientos envía token y cuenta", async () =>
        {
            var s = Preparar("""{"estado":0,"mensaje":"ok","cuenta":{"numeroCuenta":"00100001","saldo":500},"movimientos":[]}""");
            SesionEmpleado();
            var r = await new MovimientoController().ConsultarAsync("00100001");
            Afirmar.Igual(Base + "/Movimiento/consultarMovimientos", s.Url);
            Afirmar.Igual("00100001", s.Json.GetProperty("cuenta").GetString());
            Afirmar.Igual(500m, r.Saldo);
        });

        e.Caso("registrarDeposito: importe como número y sin clave", async () =>
        {
            var s = Preparar("""{"estado":0,"mensaje":"Depósito registrado correctamente","numeroMovimiento":5,"nuevoSaldo":620.50}""");
            SesionEmpleado();
            var r = await new MovimientoController().DepositoAsync("00100001", 120.50m);
            Afirmar.Igual(Base + "/Movimiento/registrarDeposito", s.Url);
            Afirmar.Igual(JsonValueKind.Number, s.Json.GetProperty("importe").ValueKind);
            Afirmar.Igual(120.50m, s.Json.GetProperty("importe").GetDecimal());
            Afirmar.Falso(s.Json.TryGetProperty("clave", out _), "sin clave");
            Afirmar.Igual(5, r.NumeroMovimiento);
            Afirmar.Igual(620.50m, r.NuevoSaldo);
        });

        e.Caso("registrarRetiro del cliente envía la clave vacía", async () =>
        {
            var s = Preparar();
            ConfigHTTP.Token = "tok-c";
            await new MovimientoController().RetiroAsync("00100001", 40m, "");
            Afirmar.Igual(Base + "/Movimiento/registrarRetiro", s.Url);
            Afirmar.Igual("", s.Json.GetProperty("clave").GetString());
            Afirmar.Igual("tok-c", s.Json.GetProperty("token").GetString());
        });

        e.Caso("registrarTransferencia envía las dos cuentas y la clave", async () =>
        {
            var s = Preparar();
            SesionEmpleado();
            await new MovimientoController().TransferenciaAsync("00100002", "00100001", 200m, "654321");
            Afirmar.Igual(Base + "/Movimiento/registrarTransferencia", s.Url);
            Afirmar.Igual("00100002", s.Json.GetProperty("cuentaOrigen").GetString());
            Afirmar.Igual("00100001", s.Json.GetProperty("cuentaDestino").GetString());
            Afirmar.Igual("654321", s.Json.GetProperty("clave").GetString());
        });

        e.Caso("Operación con token vencido se reconoce como sesión expirada", async () =>
        {
            Preparar("""{"estado":1,"mensaje":"Token inválido o sesión expirada"}""");
            var r = await new MovimientoController().RetiroAsync("00100001", 1m, "");
            Afirmar.Verdadero(r.SesionExpirada(), "sesión expirada");
        });

        return e;
    }
}
