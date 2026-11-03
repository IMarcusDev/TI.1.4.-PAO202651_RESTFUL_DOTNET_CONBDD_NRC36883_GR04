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
    public async Task<MovimientoResponseDTO> DepositoAsync(
        string cuentaOrigen,
        string cuentaDestino,
        double valor)
    {
      RESTfulClient client = new RESTfulClient();

      var body = new
      {
        token = ConfigHTTP.Token,
        cuentaOrigen = cuentaOrigen,
        cuentaDestino = cuentaDestino,
        valor = valor
      };

      try
      {
        string json = await client.PostAsync(
            Endpoints.Deposito,
            JsonSerializer.Serialize(body)
        );

        MovimientoResponseDTO dto = MovimientoResponseDTO.FromJSON(json);

        return dto;
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO(
            EstadosEnum.ERROR,
            "Error al registrar depósito: " + e.Message,
            0
        );
      }
    }

    public async Task<MovimientoResponseDTO> RetiroAsync(
        string cuentaOrigen,
        string cuentaDestino,
        double valor)
    {
      RESTfulClient client = new RESTfulClient();

      var body = new
      {
        token = ConfigHTTP.Token,
        cuentaOrigen = cuentaOrigen,
        cuentaDestino = cuentaDestino,
        valor = valor
      };

      try
      {
        string json = await client.PostAsync(
            Endpoints.Retiro,
            JsonSerializer.Serialize(body)
        );

        MovimientoResponseDTO dto = MovimientoResponseDTO.FromJSON(json);

        return dto;
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO(
            EstadosEnum.ERROR,
            "Error al registrar retiro: " + e.Message,
            0
        );
      }
    }

    public async Task<MovimientoResponseDTO> TransferenciaAsync(
        string cuentaOrigen,
        string cuentaDestino,
        double valor)
    {
      RESTfulClient client = new RESTfulClient();

      var body = new
      {
        token = ConfigHTTP.Token,
        cuentaOrigen = cuentaOrigen,
        cuentaDestino = cuentaDestino,
        valor = valor
      };

      try
      {
        string json = await client.PostAsync(
            Endpoints.Transferencia,
            JsonSerializer.Serialize(body)
        );

        MovimientoResponseDTO dto = MovimientoResponseDTO.FromJSON(json);

        return dto;
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO(
            EstadosEnum.ERROR,
            "Error al registrar transferencia: " + e.Message,
            0
        );
      }
    }
  }
}
