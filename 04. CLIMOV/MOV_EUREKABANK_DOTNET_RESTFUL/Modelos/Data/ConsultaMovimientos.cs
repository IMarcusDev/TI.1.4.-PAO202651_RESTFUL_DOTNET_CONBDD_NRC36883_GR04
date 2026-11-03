namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Data;

public sealed record ConsultaMovimientos(Cuenta Cuenta, IReadOnlyList<Movimiento> Movimientos);
