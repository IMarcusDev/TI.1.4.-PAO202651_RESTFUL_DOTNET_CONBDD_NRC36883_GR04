//revisado y aprovado por Marcos Escobar
using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.DTO;
using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.Enums;
using CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.DTO;

namespace CONSOLE_EUREKABANK_DOTNET_RESTFul.Pruebas;

internal static class PruebasDTO
{
    public static Ejecutor Crear()
    {
        var e = new Ejecutor();

        e.Caso("estado como número del enum (.NET) o como texto (Java)", () =>
        {
            Afirmar.Igual(EstadosEnum.OK, ResponseDTO.FromJSON("""{"estado":0,"mensaje":"x"}""").GetEstado());
            Afirmar.Igual(EstadosEnum.ERROR, ResponseDTO.FromJSON("""{"estado":1,"mensaje":"x"}""").GetEstado());
            Afirmar.Igual(EstadosEnum.OK, ResponseDTO.FromJSON("""{"estado":"OK","mensaje":"x"}""").GetEstado());
            Afirmar.Igual(EstadosEnum.ERROR, ResponseDTO.FromJSON("""{"estado":"ERROR"}""").GetEstado());
        });

        e.Caso("Token inválido se reconoce como sesión expirada", () =>
        {
            Afirmar.Verdadero(ResponseDTO.FromJSON("""{"estado":1,"mensaje":"Token inválido o sesión expirada"}""").SesionExpirada(), "token");
            Afirmar.Falso(ResponseDTO.FromJSON("""{"estado":1,"mensaje":"Saldo insuficiente"}""").SesionExpirada(), "negocio");
        });

        e.Caso("Login trae token, perfil y nombre", () =>
        {
            var dto = LoginResponseDTO.FromJSON("""{"estado":0,"mensaje":"Ingreso de sesión exitoso","token":"t-1","perfil":"EMPLEADO","nombre":"Carla Andrade"}""");
            Afirmar.Igual("t-1", dto.GetToken());
            Afirmar.Igual("EMPLEADO", dto.perfil);
            Afirmar.Igual("Carla Andrade", dto.nombre);
        });

        e.Caso("Operación trae número de movimiento y nuevo saldo (número o texto)", () =>
        {
            var dto = MovimientoResponseDTO.FromJSON("""{"estado":0,"mensaje":"ok","idMovimiento":23,"numeroMovimiento":23,"nuevoSaldo":475.00}""");
            Afirmar.Igual(23, dto.NumeroMovimiento);
            Afirmar.Igual(475.00m, dto.NuevoSaldo);
            var texto = MovimientoResponseDTO.FromJSON("""{"estado":"OK","mensaje":"ok","numeroMovimiento":"7","nuevoSaldo":"10.50"}""");
            Afirmar.Igual(7, texto.NumeroMovimiento);
            Afirmar.Igual(10.50m, texto.NuevoSaldo);
        });

        e.Caso("Consulta: cabecera de la cuenta y movimientos", () =>
        {
            var dto = ConsultaResponseDTO.FromJSON("""
                {"estado":0,"mensaje":"Consulta de movimientos exitosa",
                 "cuenta":{"numeroCuenta":"00100001","titular":"Ana Torres","moneda":"USD","saldo":575.00,"estadoCuenta":"ACTIVO"},
                 "movimientos":[
                   {"numeroMovimiento":23,"fecha":"2026-11-03T16:07:05","tipoMovimiento":"Transferencia","accion":"SALIDA","importe":25.00,"cuentaReferencia":"00100002"},
                   {"numeroMovimiento":24,"fecha":"2026-11-03T16:09:00","tipoMovimiento":"Depósito","accion":"INGRESO","importe":100.00,"cuentaReferencia":""}]}
                """);
            Afirmar.Igual("00100001", dto.NumeroCuenta);
            Afirmar.Igual("Ana Torres", dto.Titular);
            Afirmar.Igual(575.00m, dto.Saldo);
            Afirmar.Igual("ACTIVO", dto.EstadoCuenta);
            Afirmar.Igual(2, dto.Movimientos.Count);
            Afirmar.Igual("00100002", dto.Movimientos[0].CuentaReferencia);
            Afirmar.Falso(dto.Movimientos[0].EsIngreso(), "salida");
            Afirmar.Verdadero(dto.Movimientos[1].EsIngreso(), "ingreso");
        });

        e.Caso("Consulta sin movimientos y error sin datos", () =>
        {
            var vacia = ConsultaResponseDTO.FromJSON("""{"estado":0,"mensaje":"ok","cuenta":{"numeroCuenta":"00100002"},"movimientos":[]}""");
            Afirmar.Igual(0, vacia.Movimientos.Count);
            var error = ConsultaResponseDTO.FromJSON("""{"estado":1,"mensaje":"La cuenta no existe"}""");
            Afirmar.Igual(EstadosEnum.ERROR, error.GetEstado());
            Afirmar.Igual("La cuenta no existe", error.GetMensaje());
        });

        return e;
    }
}
