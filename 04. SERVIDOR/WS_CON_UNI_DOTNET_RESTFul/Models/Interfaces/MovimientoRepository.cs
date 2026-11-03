using WS_CON_UNI_DOTNET_RESTFul.Models.Data;

namespace WS_CON_UNI_DOTNET_RESTFul.Models.Interfaces
{
  public interface IMovimientoRepository
  {
    public abstract List<Movimiento> GetMovimientosByUserId(string id);
    public abstract Movimiento RegisterMovimiento(Movimiento m);
  }
}
