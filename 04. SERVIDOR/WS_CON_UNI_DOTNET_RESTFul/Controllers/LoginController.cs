using WS_CON_UNI_DOTNET_RESTFul.Models.Data;
using WS_CON_UNI_DOTNET_RESTFul.Models.Interfaces;
using WS_CON_UNI_DOTNET_RESTFul.Models.Repositories;

namespace WS_CON_UNI_DOTNET_RESTFul.Controllers
{
  public class LoginController
  {
    private static readonly ILoginRepository repository = new LoginImplRepository();

    public static Usuario GetUsuarioById(string id)
    {
      return LoginController.repository.GetUsuario(id);
    }

    public static Perfil GetPerfil(string username)
    {
      return LoginController.repository.GetPerfil(username);
    }
  }
}