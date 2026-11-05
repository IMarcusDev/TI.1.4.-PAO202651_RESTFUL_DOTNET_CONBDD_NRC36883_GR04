using System.Globalization;
using System.Text.Json.Nodes;

namespace ec.edu.monster.fakes;

/// <summary>
/// JSON bodies shaped like the ones WS_EUREKABANK_DOTNET_RESTFul returns: camelCase fields and
/// estado as the enum number (0 = OK, 1 = ERROR), always with HTTP 200.
/// </summary>
public static class RestRespuestas
{
    public const int Ok = 0;
    public const int Error_ = 1;

    public static string Exito(string mensaje = "Operación exitosa")
        => new JsonObject { ["estado"] = Ok, ["mensaje"] = mensaje }.ToJsonString();

    public static string LoginOk(string nombre, string tipoUsuario, string token = "token-123")
        => new JsonObject
        {
            ["estado"] = Ok,
            ["mensaje"] = "Ingreso de sesión exitoso",
            ["token"] = token,
            ["nombre"] = nombre,
            ["estadoUsuario"] = "ACTIVO",
            ["tipoUsuario"] = tipoUsuario,
            ["perfil"] = tipoUsuario.ToUpperInvariant(),
        }.ToJsonString();

    public static string Error(string mensaje)
        => new JsonObject { ["estado"] = Error_, ["mensaje"] = mensaje }.ToJsonString();

    public static string MovimientoOk(string mensaje, int numero, string nuevoSaldo)
        => new JsonObject
        {
            ["estado"] = Ok,
            ["mensaje"] = mensaje,
            ["idMovimiento"] = numero,
            ["numeroMovimiento"] = numero,
            ["nuevoSaldo"] = Numero(nuevoSaldo),
        }.ToJsonString();

    public static string ConsultaOk(params (int Numero, string Fecha, string Tipo, string Accion, string Importe, string Referencia)[] movimientos)
        => new JsonObject
        {
            ["estado"] = Ok,
            ["mensaje"] = "Consulta de movimientos exitosa",
            ["cuenta"] = new JsonObject
            {
                ["numeroCuenta"] = "00100001",
                ["titular"] = "Ana Pérez",
                ["moneda"] = "USD",
                ["saldo"] = 379.50m,
                ["estadoCuenta"] = "ACTIVO",
            },
            ["movimientos"] = new JsonArray(movimientos.Select(m => (JsonNode)new JsonObject
            {
                ["numeroMovimiento"] = m.Numero,
                ["fecha"] = m.Fecha,
                ["tipoMovimiento"] = m.Tipo,
                ["accion"] = m.Accion,
                ["importe"] = Numero(m.Importe),
                ["cuentaReferencia"] = string.IsNullOrEmpty(m.Referencia) ? null : m.Referencia,
            }).ToArray()),
        }.ToJsonString();

    private static decimal Numero(string texto) => decimal.Parse(texto, CultureInfo.InvariantCulture);
}
