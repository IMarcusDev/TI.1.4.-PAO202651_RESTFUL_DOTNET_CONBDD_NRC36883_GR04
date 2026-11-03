using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Data;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Enums;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Repositorios;
using System.Text.Json.Nodes;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Pruebas;

public static class PruebasRepositorios
{
    private const string UrlBase = "http://servidor:5088/";

    private const string Movimiento1 = """{"numeroMovimiento":1,"fecha":"2026-10-30T09:15:00","tipoMovimiento":"Depósito","accion":"INGRESO","importe":500.00,"cuentaReferencia":null}""";
    private const string Movimiento2 = """{"numeroMovimiento":2,"fecha":"2026-10-31T16:40:00","tipoMovimiento":"Transferencia","accion":"SALIDA","importe":120.50,"cuentaReferencia":"00100002"}""";

    private static LoginRestRepositorio Login(TransporteFalso transporte, bool sinRed = false)
        => new(UrlBase, transporte, new RedFalsa(sinRed));

    private static MovimientoRestRepositorio Movimientos(TransporteFalso transporte)
        => new(UrlBase, transporte, new RedFalsa(false));

    public static Ejecutor Crear()
    {
        var e = new Ejecutor();

        e.Caso("Login/loginEmpleado OK: token, perfil y nombre; ruta y cuerpo JSON", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"Ingreso de sesión exitoso","token":"abc-123","perfil":"EMPLEADO","nombre":"Marcos Escobar"}""");

            Resultado<Sesion> r = await Login(transporte).LoginEmpleadoAsync("mescobar", "secreta");

            Afirmar.Verdadero(r.Exito, r.Mensaje);
            Afirmar.Igual("abc-123", r.Datos!.Token);
            Afirmar.Igual(Perfil.Empleado, r.Datos.Perfil);
            Afirmar.Igual("Marcos Escobar", r.Datos.Nombre);
            Afirmar.Igual(UrlBase + "Login/loginEmpleado", transporte.Ultima.Url);
            Afirmar.Igual("""{"usuario":"mescobar","clave":"secreta"}""", transporte.Ultima.Json);
        });
        e.Caso("Login/loginCliente guarda la cuenta de la sesión", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"ok","token":"t","perfil":"CLIENTE","nombre":"Ana"}""");

            Resultado<Sesion> r = await Login(transporte).LoginClienteAsync("00100001", "123456");

            Afirmar.Igual(Perfil.Cliente, r.Datos!.Perfil);
            Afirmar.Igual("00100001", r.Datos.Cuenta);
            Afirmar.Igual(UrlBase + "Login/loginCliente", transporte.Ultima.Url);
            Afirmar.Igual("""{"cuenta":"00100001","clave":"123456"}""", transporte.Ultima.Json);
        });
        e.Caso("Login con estado numérico de .NET (0 = OK) y nombres en PascalCase", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"Estado":0,"Mensaje":"ok","Token":"t","Perfil":"EMPLEADO","Nombre":"N"}""");

            Resultado<Sesion> r = await Login(transporte).LoginEmpleadoAsync("u", "c");

            Afirmar.Verdadero(r.Exito, r.Mensaje);
            Afirmar.Igual("N", r.Datos!.Nombre);
        });
        e.Caso("Login con credenciales incorrectas (401) muestra el mensaje del servidor", async () =>
        {
            var transporte = TransporteFalso.Responde(401, """{"estado":1,"mensaje":"La cuenta no se encuentra activa"}""");

            Resultado<Sesion> r = await Login(transporte).LoginClienteAsync("00100001", "123456");

            Afirmar.Falso(r.Exito, "debía fallar");
            Afirmar.Igual("La cuenta no se encuentra activa", r.Mensaje);
        });
        e.Caso("Login OK sin token es respuesta incompleta", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"ok","token":""}""");

            Afirmar.Igual(RestErrores.RespuestaIncompleta, (await Login(transporte).LoginEmpleadoAsync("u", "c")).Mensaje);
        });
        e.Caso("Ruta inexistente (404 sin JSON) informa el código HTTP", async () =>
        {
            var transporte = TransporteFalso.Responde(404, TransporteFalso.HtmlErrorKestrel);

            Afirmar.Contiene("HTTP 404", (await Login(transporte).LoginEmpleadoAsync("u", "c")).Mensaje);
        });
        e.Caso("Login sin respuesta en 15 s informa que no hay servidor", async () =>
        {
            var transporte = TransporteFalso.Lanza(new TaskCanceledException("timeout"));

            Afirmar.EmpiezaCon(RestErrores.SinServidor, (await Login(transporte).LoginEmpleadoAsync("u", "c")).Mensaje);
        });
        e.Caso("Login sin red no llama al servidor", async () =>
        {
            var transporte = TransporteFalso.Responde("");

            Afirmar.Igual(RestErrores.SinConexion, (await Login(transporte, sinRed: true).LoginEmpleadoAsync("u", "c")).Mensaje);
            Afirmar.Igual(0, transporte.Peticiones.Count, "peticiones");
        });
        e.Caso("Logout envía el token", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"Sesión cerrada"}""");

            Afirmar.Verdadero((await Login(transporte).LogoutAsync("abc-123")).Exito, "logout");
            Afirmar.Igual(UrlBase + "Login/logout", transporte.Ultima.Url);
            Afirmar.Igual("""{"token":"abc-123"}""", transporte.Ultima.Json);
        });
        e.Caso("Movimiento/consultarMovimientos lee la cuenta y sus movimientos", async () =>
        {
            var transporte = TransporteFalso.Responde($$"""
                {"estado":"OK","mensaje":"Consulta de movimientos exitosa",
                 "cuenta":{"numeroCuenta":"00100001","titular":"Ana Pérez","moneda":"USD","saldo":379.50,"estadoCuenta":"ACTIVO"},
                 "movimientos":[{{Movimiento1}},{{Movimiento2}}]}
                """);

            Resultado<ConsultaMovimientos> r = await Movimientos(transporte).ConsultarMovimientosAsync("tok", "00100001");

            Afirmar.Verdadero(r.Exito, r.Mensaje);
            Afirmar.Igual("Ana Pérez", r.Datos!.Cuenta.Titular);
            Afirmar.Igual(379.50m, r.Datos.Cuenta.Saldo);
            Afirmar.Igual(2, r.Datos.Movimientos.Count);
            Afirmar.Igual("1", r.Datos.Movimientos[0].Numero);
            Afirmar.Falso(r.Datos.Movimientos[0].TieneReferencia, "sin referencia");
            Movimiento transferencia = r.Datos.Movimientos[1];
            Afirmar.Falso(transferencia.EsIngreso, "es salida");
            Afirmar.Igual("00100002", transferencia.CuentaReferencia);
            Afirmar.Igual(UrlBase + "Movimiento/consultarMovimientos", transporte.Ultima.Url);
            Afirmar.Igual("""{"token":"tok","cuenta":"00100001"}""", transporte.Ultima.Json);
        });
        e.Caso("Movimiento/consultarMovimientos de una cuenta sin movimientos devuelve lista vacía", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"ok","cuenta":{"numeroCuenta":"00100001"},"movimientos":[]}""");

            Afirmar.Igual(0, (await Movimientos(transporte).ConsultarMovimientosAsync("tok", "00100001")).Datos!.Movimientos.Count);
        });
        e.Caso("Token inválido (401) marca la sesión como expirada", async () =>
        {
            var transporte = TransporteFalso.Responde(401, """{"estado":"ERROR","mensaje":"Token inválido o sesión expirada"}""");

            Afirmar.Verdadero((await Movimientos(transporte).ConsultarMovimientosAsync("tok", "00100001")).SesionExpirada, "expirada");
        });
        e.Caso("Movimiento/registrarTransferencia envía las dos cuentas, el importe como número y la clave", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"Transferencia registrada correctamente","numeroMovimiento":15,"nuevoSaldo":879.50}""");
            var solicitud = new SolicitudOperacion(TipoOperacion.Transferencia, "00100001", "00100002", 120.50m, "123456");

            Resultado<Comprobante> r = await Movimientos(transporte).RegistrarAsync("tok", solicitud);

            Afirmar.Verdadero(r.Exito, r.Mensaje);
            Afirmar.Igual("15", r.Datos!.NumeroMovimiento);
            Afirmar.Igual(879.50m, r.Datos.NuevoSaldo);
            Afirmar.Igual(UrlBase + "Movimiento/registrarTransferencia", transporte.Ultima.Url);
            Afirmar.Igual("""{"token":"tok","cuentaOrigen":"00100001","cuentaDestino":"00100002","importe":120.50,"clave":"123456"}""",
                transporte.Ultima.Json);
        });
        e.Caso("Movimiento/registrarDeposito no envía clave", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"ok","numeroMovimiento":3,"nuevoSaldo":10}""");
            var solicitud = new SolicitudOperacion(TipoOperacion.Deposito, "00100001", null, 10.00m, "");

            await Movimientos(transporte).RegistrarAsync("tok", solicitud);

            JsonObject cuerpo = transporte.Ultima.Cuerpo;
            Afirmar.Igual(UrlBase + "Movimiento/registrarDeposito", transporte.Ultima.Url);
            Afirmar.Igual("00100001", (string?)cuerpo["cuenta"]);
            Afirmar.Falso(cuerpo.ContainsKey("clave"), "no envía clave");
        });
        e.Caso("Movimiento/registrarRetiro del cliente envía la clave vacía", async () =>
        {
            var transporte = TransporteFalso.Responde("""{"estado":"OK","mensaje":"ok","numeroMovimiento":4,"nuevoSaldo":5}""");
            var solicitud = new SolicitudOperacion(TipoOperacion.Retiro, "00100001", null, 5.00m, "");

            await Movimientos(transporte).RegistrarAsync("tok", solicitud);

            Afirmar.Igual(UrlBase + "Movimiento/registrarRetiro", transporte.Ultima.Url);
            Afirmar.Igual("", (string?)transporte.Ultima.Cuerpo["clave"]);
        });
        e.Caso("Error de negocio (400): se muestra el mensaje del servidor", async () =>
        {
            var transporte = TransporteFalso.Responde(400, """{"estado":"ERROR","mensaje":"Saldo insuficiente"}""");
            var solicitud = new SolicitudOperacion(TipoOperacion.Retiro, "00100001", null, 9999.00m, "123456");

            Resultado<Comprobante> r = await Movimientos(transporte).RegistrarAsync("tok", solicitud);

            Afirmar.Falso(r.Exito, "debía fallar");
            Afirmar.Falso(r.SesionExpirada, "no es sesión expirada");
            Afirmar.Igual("Saldo insuficiente", r.Mensaje);
        });
        e.Caso("Error interno (500 sin JSON) no se toma como éxito", async () =>
        {
            var transporte = TransporteFalso.Responde(500, "Internal Server Error");
            var solicitud = new SolicitudOperacion(TipoOperacion.Retiro, "00100001", null, 1.00m, "123456");

            Resultado<Comprobante> r = await Movimientos(transporte).RegistrarAsync("tok", solicitud);

            Afirmar.Falso(r.Exito, "debía fallar");
            Afirmar.Contiene("HTTP 500", r.Mensaje);
        });

        return e;
    }
}
