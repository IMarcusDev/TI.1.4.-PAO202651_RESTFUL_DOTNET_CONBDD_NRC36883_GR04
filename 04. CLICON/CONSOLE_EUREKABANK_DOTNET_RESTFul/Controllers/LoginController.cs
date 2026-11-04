using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.DTO;
using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.Enums;
using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.HTTP;
using CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.Data;
using CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.HTTP;
using System.Text.Json;

namespace CONSOLE_EUREKABANK_DOTNET_RESTFul.Controllers
{
  internal class LoginController
  {
    //revisado y aprovado por Marcos Escobar
    // ECUD1: el empleado ingresa con su usuario y clave.
    public async Task<LoginResponseDTO> LoginEmpleadoAsync(
        string usuario,
        string clave)
    {
      var body = new
      {
        usuario = usuario,
        clave = clave
      };

      return await IniciarSesionAsync(Endpoints.LoginEmpleado, body, null);
    }

    //revisado y aprovado por Marcos Escobar
    // ECUD1: el cliente ingresa con el número de su cuenta y la clave de esa cuenta.
    public async Task<LoginResponseDTO> LoginClienteAsync(
        string cuenta,
        string clave)
    {
      var body = new
      {
        cuenta = cuenta,
        clave = clave
      };

      return await IniciarSesionAsync(Endpoints.LoginCliente, body, cuenta);
    }

    //revisado y aprovado por Marcos Escobar
    // Cierra la sesión en el servidor; los datos locales se borran aunque el servidor no responda.
    public async Task<ResponseDTO> LogoutAsync()
    {
      RESTfulClient client = new RESTfulClient();

      var body = new
      {
        token = ConfigHTTP.Token
      };

      try
      {
        string json = await client.PostAsync(
            Endpoints.Logout,
            JsonSerializer.Serialize(body)
        );

        return ResponseDTO.FromJSON(json);
      }
      catch (Exception e)
      {
        return new ResponseDTO(EstadosEnum.ERROR, ErroresCliente.Mensaje(e));
      }
      finally
      {
        ConfigHTTP.Limpiar();
      }
    }

    //revisado y aprovado por Marcos Escobar
    private async Task<LoginResponseDTO> IniciarSesionAsync(string endpoint, object body, string? cuenta)
    {
      RESTfulClient client = new RESTfulClient();

      try
      {
        string json = await client.PostAsync(
            endpoint,
            JsonSerializer.Serialize(body)
        );

        LoginResponseDTO dto = LoginResponseDTO.FromJSON(json);

        if (dto.GetEstado() == EstadosEnum.OK)
        {
          ConfigHTTP.Token = dto.GetToken();
          ConfigHTTP.Perfil = dto.perfil;
          ConfigHTTP.Nombre = dto.nombre;
          ConfigHTTP.Cuenta = cuenta ?? "";
        }

        return dto;
      }
      catch (Exception e)
      {
        return new LoginResponseDTO(
            EstadosEnum.ERROR,
            ErroresCliente.Mensaje(e),
            ""
        );
      }
    }
  }

}
