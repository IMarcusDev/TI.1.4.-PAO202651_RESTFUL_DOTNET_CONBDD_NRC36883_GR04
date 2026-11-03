using WS_CON_UNI_DOTNET_RESTFul.Models.Data;

namespace WS_CON_UNI_DOTNET_RESTFul.Models.Interfaces
{
  public interface ICuentaRepository
  {
    public abstract Cuenta GetCuenta(string id);
    public abstract Cuenta UpdateCuenta(Cuenta c);
  }
}
