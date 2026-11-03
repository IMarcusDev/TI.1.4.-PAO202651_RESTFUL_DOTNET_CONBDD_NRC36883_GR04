using CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.HTTP;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Models.Data;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTFul.Models.HTTP;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTFul.Controllers
{
  internal class MovimientoController
  {
    //revisado y aprovado por Marcos Escobar
    // ECUD2: datos de la cuenta y lista de sus movimientos.
    public async Task<ConsultaResponseDTO> ConsultarAsync(string cuenta)
    {
      RESTfulClient client = new RESTfulClient();

      var body = new
      {
        token = ConfigHTTP.Token,
        cuenta = cuenta
      };

      try
      {
        string json = await client.PostAsync(
            Endpoints.Consultar,
            JsonSerializer.Serialize(body)
        );

        return ConsultaResponseDTO.FromJSON(json);
      }
      catch (Exception e)
      {
        return new ConsultaResponseDTO(EstadosEnum.ERROR, ErroresCliente.Mensaje(e));
      }
    }

    //revisado y aprovado por Marcos Escobar
    // ECUD3: el depósito lo registra el empleado de la sesión; no lleva clave.
    public async Task<MovimientoResponseDTO> DepositoAsync(
        string cuenta,
        decimal importe)
    {
      var body = new
      {
        token = ConfigHTTP.Token,
        cuenta = cuenta,
        importe = importe
      };

      return await RegistrarAsync(Endpoints.Deposito, body);
    }

    //revisado y aprovado por Marcos Escobar
    // ECUD3: el empleado envía la clave de la cuenta; el cliente opera su cuenta y la clave va vacía.
    public async Task<MovimientoResponseDTO> RetiroAsync(
        string cuenta,
        decimal importe,
        string clave)
    {
      var body = new
      {
        token = ConfigHTTP.Token,
        cuenta = cuenta,
        importe = importe,
        clave = clave
      };

      return await RegistrarAsync(Endpoints.Retiro, body);
    }

    //revisado y aprovado por Marcos Escobar
    public async Task<MovimientoResponseDTO> TransferenciaAsync(
        string cuentaOrigen,
        string cuentaDestino,
        decimal importe,
        string clave)
    {
      var body = new
      {
        token = ConfigHTTP.Token,
        cuentaOrigen = cuentaOrigen,
        cuentaDestino = cuentaDestino,
        importe = importe,
        clave = clave
      };

      return await RegistrarAsync(Endpoints.Transferencia, body);
    }

    //revisado y aprovado por Marcos Escobar
    private async Task<MovimientoResponseDTO> RegistrarAsync(string endpoint, object body)
    {
      RESTfulClient client = new RESTfulClient();

      try
      {
        string json = await client.PostAsync(
            endpoint,
            JsonSerializer.Serialize(body)
        );

        return MovimientoResponseDTO.FromJSON(json);
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO(
            EstadosEnum.ERROR,
            ErroresCliente.Mensaje(e),
            0
        );
      }
    }
  }
}
