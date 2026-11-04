namespace WS_EUREKABANK_DOTNET_RESTFul.Models.DTO
{
  /// <summary>
  /// Cuerpo de las operaciones del ERS en el recurso Movimiento:
  /// consultarMovimientos {token, cuenta}, registrarDeposito {token, cuenta, importe},
  /// registrarRetiro {token, cuenta, importe, clave} y
  /// registrarTransferencia {token, cuentaOrigen, cuentaDestino, importe, clave}.
  /// </summary>
  //revisado y aprovado por Marcos Escobar
  public class OperacionRequestDTO
  {
    public string? Token { get; set; }
    public string? Cuenta { get; set; }
    public string? CuentaOrigen { get; set; }
    public string? CuentaDestino { get; set; }
    public decimal? Importe { get; set; }
    public string? Clave { get; set; }
  }
}
