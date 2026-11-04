namespace WS_EUREKABANK_DOTNET_RESTFul.Models.DTO
{
  /// <summary>
  /// Cuerpo de Login/loginEmpleado {usuario, clave}, Login/loginCliente {cuenta, clave}
  /// y Login/logout {token}. Los campos son opcionales para que [ApiController] no rechace
  /// el cuerpo antes de llegar a las validaciones del servidor.
  /// </summary>
  //revisado y aprovado por Marcos Escobar
  public class CredencialesRequestDTO
  {
    public string? Usuario { get; set; }
    public string? Cuenta { get; set; }
    public string? Clave { get; set; }
    public string? Token { get; set; }
  }
}
