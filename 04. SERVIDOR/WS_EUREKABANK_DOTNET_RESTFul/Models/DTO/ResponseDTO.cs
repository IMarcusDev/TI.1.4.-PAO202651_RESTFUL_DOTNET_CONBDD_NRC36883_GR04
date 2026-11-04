using WS_EUREKABANK_DOTNET_RESTFul.Models.Enums;

namespace WS_EUREKABANK_DOTNET_RESTFul.Models.DTO
{
  public class ResponseDTO
  {
    public EstadosEnum Estado { get; set; }
    public string Mensaje { get; set; } = "";
  }
}
