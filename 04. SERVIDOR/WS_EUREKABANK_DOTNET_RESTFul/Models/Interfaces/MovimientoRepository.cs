using WS_EUREKABANK_DOTNET_RESTFul.Models.Data;
//revisado y aprovado por Marcos Escobar
using Microsoft.Data.SqlClient;

namespace WS_EUREKABANK_DOTNET_RESTFul.Models.Interfaces
{
  public interface IMovimientoRepository
  {
    public abstract List<Movimiento> GetMovimientosByUserId(string id);
    //revisado y aprovado por Marcos Escobar
    public abstract Movimiento RegisterMovimiento(SqlConnection connection, SqlTransaction transaction, Movimiento m);
  }
}
