using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.DTO;
using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CONSOLE_EUREKABANK_DOTNET_RESTFul.Models.DTO
{
  internal class MovimientoResponseDTO : ResponseDTO
  {
    public int IdMovimiento { get; set; }
    //revisado y aprovado por Marcos Escobar
    // Datos del comprobante (RNF-010): número de movimiento y nuevo saldo.
    public int NumeroMovimiento { get; set; }
    public decimal? NuevoSaldo { get; set; }

    public MovimientoResponseDTO(EstadosEnum estado, string mensaje, int idMovimiento) : base(estado, mensaje)
    {
      this.IdMovimiento = idMovimiento;
    }

    //revisado y aprovado por Marcos Escobar
    public static new MovimientoResponseDTO FromJSON(string json)
    {
      ResponseDTO response = ResponseDTO.FromJSON(json);

      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement obj = doc.RootElement;

      int idMovimiento = LeerEntero(obj, "idMovimiento");

      return new MovimientoResponseDTO(
        response.GetEstado(),
        response.GetMensaje(),
        idMovimiento
      )
      {
        NumeroMovimiento = LeerEntero(obj, "numeroMovimiento"),
        NuevoSaldo = LeerDecimal(obj, "nuevoSaldo")
      };
    }

  }
}
