namespace WS_CON_UNI_DOTNET_RESTFul.Models.DTO
{
  public class MovimientoRequestDTO
  {
    public string Token { get; set; } = string.Empty;
    public string CuentaOrigen { get; set; } = string.Empty;
    public string CuentaDestino { get; set; } = string.Empty;
    public double Valor { get; set; }
  }
}