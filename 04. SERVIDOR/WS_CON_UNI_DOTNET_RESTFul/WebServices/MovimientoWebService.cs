using WS_CON_UNI_DOTNET_RESTFul.Controllers;
using WS_CON_UNI_DOTNET_RESTFul.Models.Data;
using WS_CON_UNI_DOTNET_RESTFul.Models.DTO;
using WS_CON_UNI_DOTNET_RESTFul.Models.Enums;
using WS_CON_UNI_DOTNET_RESTFul.Services;
using Microsoft.AspNetCore.Mvc;

namespace WS_CON_UNI_DOTNET_RESTFul.WebServices
{
  [ApiController]
  [Route("Movimiento")]
  public class MovimientoWebService : ControllerBase
  {
    [HttpPost("Deposito")]
    public MovimientoResponseDTO Deposito(
      [FromBody] MovimientoRequestDTO request
    )
    {
      if (!TokenService.ValidateToken(request.Token))
        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = "Token inválido"
        };

      try
      {
        Movimiento movimiento = MovimientoController.RegisterDeposito(
          new Movimiento
          {
            Tipo = "DEPOSITO",
            CuentaOrigen = request.CuentaOrigen,
            CuentaDestino = request.CuentaDestino,
            Valor = request.Valor
          }
        );

        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.OK,
          Mensaje = "Depósito registrado exitosamente",
          IdMovimiento = int.Parse(movimiento.Id)
        };
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = e.Message
        };
      }
    }

    [HttpPost("Retiro")]
    public MovimientoResponseDTO Retiro(
      [FromBody] MovimientoRequestDTO request
    )
    {
      if (!TokenService.ValidateToken(request.Token))
        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = "Token inválido"
        };

      try
      {
        Movimiento movimiento = MovimientoController.RegisterRetiro(
          new Movimiento
          {
            Tipo = "RETIRO",
            CuentaOrigen = request.CuentaOrigen,
            CuentaDestino = request.CuentaDestino,
            Valor = request.Valor
          }
        );

        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.OK,
          Mensaje = "Retiro registrado exitosamente",
          IdMovimiento = int.Parse(movimiento.Id)
        };
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = e.Message
        };
      }
    }

    [HttpPost("Transferencia")]
    public MovimientoResponseDTO Transferencia(
      [FromBody] MovimientoRequestDTO request
    )
    {
      if (!TokenService.ValidateToken(request.Token))
        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = "Token inválido"
        };

      try
      {
        Movimiento movimiento = MovimientoController.RegisterTransferencia(
          new Movimiento
          {
            Tipo = "TRANSFERENCIA",
            CuentaOrigen = request.CuentaOrigen,
            CuentaDestino = request.CuentaDestino,
            Valor = request.Valor
          }
        );

        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.OK,
          Mensaje = "Transferencia registrada exitosamente",
          IdMovimiento = int.Parse(movimiento.Id)
        };
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = e.Message
        };
      }
    }
  }
}