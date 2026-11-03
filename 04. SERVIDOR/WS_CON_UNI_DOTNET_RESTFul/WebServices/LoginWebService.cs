using WS_CON_UNI_DOTNET_RESTFul.Controllers;
using WS_CON_UNI_DOTNET_RESTFul.Models.Data;
using WS_CON_UNI_DOTNET_RESTFul.Models.DTO;
using WS_CON_UNI_DOTNET_RESTFul.Models.Enums;
using WS_CON_UNI_DOTNET_RESTFul.Services;
using Microsoft.AspNetCore.Mvc;

namespace WS_CON_UNI_DOTNET_RESTFul.WebServices
{
  [ApiController]
  [Route("Login")]
  public class LoginWebService : ControllerBase
  {
    [HttpPost]
    public LoginResponseDTO Login([FromBody] LoginRequestDTO request)
    {
      try
      {
        Perfil p = LoginController.GetPerfil(request.Username);
        Usuario u = LoginController.GetUsuarioById(p.Id);

        if (!HashService.VerifyPassword(request.Password, p.PasswordHash))
          throw new Exception("Contraseña incorrecta");

        string token = TokenService.GenerateToken();

        return new LoginResponseDTO
        {
          Estado = EstadosEnum.OK,
          Mensaje = "Ingreso de Sesión Exitoso!",
          Token = token,
          Nombre = u.Nombre,
          EstadoUsuario = u.Estado,
          TipoUsuario = u.Tipo
        };
      }
      catch (Exception e)
      {
        return new LoginResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = e.Message
        };
      }
    }
  }
}