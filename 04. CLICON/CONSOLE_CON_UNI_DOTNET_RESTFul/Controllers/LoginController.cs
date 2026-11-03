using CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.HTTP;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Models.Data;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Models.HTTP;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTFul.Controllers
{
  internal class LoginController
  {
    public async Task<LoginResponseDTO> LoginAsync(
        string username,
        string password)
    {
      RESTfulClient client = new RESTfulClient();

      var body = new
      {
        username = username,
        password = password
      };

      try
      {
        string json = await client.PostAsync(
            Endpoints.Login,
            JsonSerializer.Serialize(body)
        );

        LoginResponseDTO dto = LoginResponseDTO.FromJSON(json);

        ConfigHTTP.Token = dto.GetToken();

        return dto;
      }
      catch (Exception e)
      {
        return new LoginResponseDTO(
            EstadosEnum.ERROR,
            "Error al iniciar sesión: " + e.Message,
            ""
        );
      }
    }
  }

}
