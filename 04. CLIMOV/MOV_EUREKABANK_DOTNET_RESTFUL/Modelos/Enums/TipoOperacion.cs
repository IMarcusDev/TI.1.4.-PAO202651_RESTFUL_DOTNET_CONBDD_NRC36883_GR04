namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Enums;

public enum TipoOperacion
{
    Deposito,
    Retiro,
    Transferencia,
}

public static class TipoOperacionExtensions
{
    /// <summary>Ruta de la operación dentro del recurso Movimiento (por ejemplo, Movimiento/registrarDeposito).</summary>
    public static string Ruta(this TipoOperacion tipo) => tipo switch
    {
        TipoOperacion.Deposito => "registrarDeposito",
        TipoOperacion.Retiro => "registrarRetiro",
        _ => "registrarTransferencia",
    };
}
