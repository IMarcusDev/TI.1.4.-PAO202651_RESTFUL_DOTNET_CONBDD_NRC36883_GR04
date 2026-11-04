using WS_EUREKABANK_DOTNET_RESTFul.Controllers;
using WS_EUREKABANK_DOTNET_RESTFul.Models.Data;
using WS_EUREKABANK_DOTNET_RESTFul.Models.DTO;
using WS_EUREKABANK_DOTNET_RESTFul.Models.Enums;
using WS_EUREKABANK_DOTNET_RESTFul.Services;
using Microsoft.AspNetCore.Mvc;

namespace WS_EUREKABANK_DOTNET_RESTFul.WebServices
{
  [ApiController]
  [Route("Login")]
  public class LoginWebService : ControllerBase
  {
    //revisado y aprovado por Marcos Escobar
    // Ya no indica si falló el usuario o la clave, revisa que el usuario esté activo
    // y el token queda asociado al perfil y al código del usuario.
    [HttpPost]
    public LoginResponseDTO Login([FromBody] LoginRequestDTO request)
    {
      try
      {
        Usuario u = LoginController.Autenticar(request.Username, request.Password);

        string token = TokenService.GenerateToken(LoginController.PerfilDe(u), u.Id);

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

    //revisado y aprovado por Marcos Escobar
    [HttpPost("loginEmpleado")]
    public LoginResponseDTO LoginEmpleado([FromBody] CredencialesRequestDTO request)
    {
      try
      {
        Usuario u = LoginController.AutenticarEmpleado(request.Usuario, request.Clave);
        return SesionIniciada(u, Sesion.EMPLEADO, u.Id);
      }
      catch (Exception e)
      {
        return new LoginResponseDTO { Estado = EstadosEnum.ERROR, Mensaje = e.Message };
      }
    }

    //revisado y aprovado por Marcos Escobar
    [HttpPost("loginCliente")]
    public LoginResponseDTO LoginCliente([FromBody] CredencialesRequestDTO request)
    {
      try
      {
        Usuario u = LoginController.AutenticarCliente(request.Cuenta, request.Clave);
        return SesionIniciada(u, Sesion.CLIENTE, request.Cuenta!);
      }
      catch (Exception e)
      {
        return new LoginResponseDTO { Estado = EstadosEnum.ERROR, Mensaje = e.Message };
      }
    }

    //revisado y aprovado por Marcos Escobar
    // Elimina el token. Responde OK aunque ya no exista, porque la sesión queda cerrada igual.
    [HttpPost("logout")]
    public LoginResponseDTO Logout([FromBody] CredencialesRequestDTO request)
    {
      TokenService.RemoveToken(request.Token);
      return new LoginResponseDTO { Estado = EstadosEnum.OK, Mensaje = "Sesión cerrada" };
    }

    //revisado y aprovado por Marcos Escobar
    private static LoginResponseDTO SesionIniciada(Usuario u, string perfil, string codigo)
    {
      return new LoginResponseDTO
      {
        Estado = EstadosEnum.OK,
        Mensaje = "Ingreso de sesión exitoso",
        Token = TokenService.GenerateToken(perfil, codigo),
        Nombre = u.Nombre,
        EstadoUsuario = u.Estado,
        TipoUsuario = u.Tipo,
        Perfil = perfil
      };
    }
  }
}
