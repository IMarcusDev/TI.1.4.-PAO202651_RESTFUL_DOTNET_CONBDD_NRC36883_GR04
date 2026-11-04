using WS_EUREKABANK_DOTNET_RESTFul.Controllers;
using WS_EUREKABANK_DOTNET_RESTFul.Models.Data;
using WS_EUREKABANK_DOTNET_RESTFul.Models.DTO;
using WS_EUREKABANK_DOTNET_RESTFul.Models.Enums;
using WS_EUREKABANK_DOTNET_RESTFul.Services;
using Microsoft.AspNetCore.Mvc;
//revisado y aprovado por Marcos Escobar
using Microsoft.Data.SqlClient;

namespace WS_EUREKABANK_DOTNET_RESTFul.WebServices
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
        //revisado y aprovado por Marcos Escobar
        MovimientoController.Autorizar(TokenService.GetSesion(request.Token), "DEPOSITO", request.CuentaOrigen);

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
        //revisado y aprovado por Marcos Escobar
        MovimientoController.Autorizar(TokenService.GetSesion(request.Token), "RETIRO", request.CuentaOrigen);

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
        //revisado y aprovado por Marcos Escobar
        MovimientoController.Autorizar(TokenService.GetSesion(request.Token), "TRANSFERENCIA", request.CuentaOrigen);

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

    //revisado y aprovado por Marcos Escobar
    [HttpPost("consultarMovimientos")]
    public ConsultaMovimientosResponseDTO ConsultarMovimientos([FromBody] OperacionRequestDTO request)
    {
      Sesion? sesion = TokenService.GetSesion(request.Token);
      if (sesion == null)
        return new ConsultaMovimientosResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = MovimientoController.TOKEN_INVALIDO
        };

      try
      {
        string cuenta = request.Cuenta ?? "";
        Cuenta datos = MovimientoController.GetCuentaConsultable(sesion, cuenta);
        Usuario titular = LoginController.GetUsuarioById(cuenta);

        return new ConsultaMovimientosResponseDTO
        {
          Estado = EstadosEnum.OK,
          Mensaje = "Consulta de movimientos exitosa",
          Cuenta = new CuentaDTO
          {
            NumeroCuenta = datos.Id,
            Titular = titular.Nombre,
            Saldo = MovimientoDTO.Monto(datos.Saldo),
            EstadoCuenta = datos.Estado
          },
          Movimientos = MovimientoController.GetMovimientosByUserId(cuenta)
            .Select(m => MovimientoDTO.Desde(m, cuenta))
            .ToList()
        };
      }
      catch (SqlException)
      {
        return new ConsultaMovimientosResponseDTO
        {
          Estado = EstadosEnum.ERROR,
          Mensaje = LoginController.SERVICIO_NO_DISPONIBLE
        };
      }
      catch (Exception e)
      {
        return new ConsultaMovimientosResponseDTO { Estado = EstadosEnum.ERROR, Mensaje = e.Message };
      }
    }

    //revisado y aprovado por Marcos Escobar
    [HttpPost("registrarDeposito")]
    public MovimientoResponseDTO RegistrarDeposito([FromBody] OperacionRequestDTO request)
    {
      return Registrar(request, "DEPOSITO", request.Cuenta, null, "Depósito registrado correctamente");
    }

    //revisado y aprovado por Marcos Escobar
    [HttpPost("registrarRetiro")]
    public MovimientoResponseDTO RegistrarRetiro([FromBody] OperacionRequestDTO request)
    {
      return Registrar(request, "RETIRO", request.Cuenta, null, "Retiro registrado correctamente");
    }

    //revisado y aprovado por Marcos Escobar
    [HttpPost("registrarTransferencia")]
    public MovimientoResponseDTO RegistrarTransferencia([FromBody] OperacionRequestDTO request)
    {
      return Registrar(
        request, "TRANSFERENCIA", request.CuentaOrigen, request.CuentaDestino,
        "Transferencia registrada correctamente"
      );
    }

    //revisado y aprovado por Marcos Escobar
    private static MovimientoResponseDTO Registrar(
      OperacionRequestDTO request,
      string tipo,
      string? cuentaOrigen,
      string? cuentaDestino,
      string mensajeOk
    )
    {
      try
      {
        Movimiento movimiento = MovimientoController.Registrar(
          TokenService.GetSesion(request.Token), tipo, cuentaOrigen, cuentaDestino, request.Importe, request.Clave
        );
        int numero = int.Parse(movimiento.Id);

        return new MovimientoResponseDTO
        {
          Estado = EstadosEnum.OK,
          Mensaje = mensajeOk,
          IdMovimiento = numero,
          NumeroMovimiento = numero,
          NuevoSaldo = MovimientoDTO.Monto(movimiento.NuevoSaldo)
        };
      }
      catch (Exception e)
      {
        return new MovimientoResponseDTO { Estado = EstadosEnum.ERROR, Mensaje = e.Message };
      }
    }
  }
}
