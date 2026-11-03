using CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO;
using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTFul.Models.DTO
{
  internal class MovimientoResponseDTO : ResponseDTO
  {
    public int IdMovimiento { get; set; }

    public MovimientoResponseDTO(EstadosEnum estado, string mensaje, int idMovimiento) : base(estado, mensaje)
    {
      this.IdMovimiento = idMovimiento;
    }

    public static new MovimientoResponseDTO FromJSON(string json)
    {
      ResponseDTO response = ResponseDTO.FromJSON(json);

      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement obj = doc.RootElement;

      int idMovimiento = obj.TryGetProperty("idMovimiento", out JsonElement idElement)
        ? idElement.GetInt32()
        : 0;

      return new MovimientoResponseDTO(
        response.GetEstado(),
        response.GetMensaje(),
        idMovimiento
      );
    }

  }
}
