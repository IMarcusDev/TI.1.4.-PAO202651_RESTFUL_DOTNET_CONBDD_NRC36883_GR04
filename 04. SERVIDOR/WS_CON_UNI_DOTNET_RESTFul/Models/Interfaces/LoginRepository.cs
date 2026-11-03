using WS_CON_UNI_DOTNET_RESTFul.Models.Data;

namespace WS_CON_UNI_DOTNET_RESTFul.Models.Interfaces
{
  public interface ILoginRepository
  {
    public abstract Perfil GetPerfil(string username);
    public abstract Usuario GetUsuario(string id);
  }
}
