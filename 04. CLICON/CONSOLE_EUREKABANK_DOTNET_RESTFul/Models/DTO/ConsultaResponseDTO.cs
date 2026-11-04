using CONSOLE_EUREKABANK_DOTNET_RESTful.Models.Enums;
using System.Text.Json;

namespace CONSOLE_EUREKABANK_DOTNET_RESTful.Models.DTO
{
  /// <summary>Respuesta de consultarMovimientos: los datos de la cuenta y la lista de sus movimientos.</summary>
  //revisado y aprovado por Marcos Escobar
  internal class ConsultaResponseDTO : ResponseDTO
  {
    public string NumeroCuenta { get; set; } = "";
    public string Titular { get; set; } = "";
    public string Moneda { get; set; } = "";
    public decimal? Saldo { get; set; }
    public string EstadoCuenta { get; set; } = "";
    public List<MovimientoDTO> Movimientos { get; set; } = new List<MovimientoDTO>();

    //revisado y aprovado por Marcos Escobar
    /// <summary>Movimientos del más reciente al más antiguo (mayor número de movimiento primero): es el orden de la tabla.</summary>
    public List<MovimientoDTO> MovimientosRecientes() => Movimientos.OrderByDescending(m => m.NumeroMovimiento).ToList();

    public ConsultaResponseDTO(EstadosEnum estado, string mensaje) : base(estado, mensaje)
    {
    }

    public static new ConsultaResponseDTO FromJSON(string json)
    {
      ResponseDTO response = ResponseDTO.FromJSON(json);

      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement obj = doc.RootElement;

      ConsultaResponseDTO dto = new ConsultaResponseDTO(response.GetEstado(), response.GetMensaje());

      if (obj.TryGetProperty("cuenta", out JsonElement cuenta) && cuenta.ValueKind == JsonValueKind.Object)
      {
        dto.NumeroCuenta = LeerTexto(cuenta, "numeroCuenta");
        dto.Titular = LeerTexto(cuenta, "titular");
        dto.Moneda = LeerTexto(cuenta, "moneda");
        dto.Saldo = LeerDecimal(cuenta, "saldo");
        dto.EstadoCuenta = LeerTexto(cuenta, "estadoCuenta");
      }

      if (obj.TryGetProperty("movimientos", out JsonElement lista) && lista.ValueKind == JsonValueKind.Array)
      {
        foreach (JsonElement m in lista.EnumerateArray())
        {
          dto.Movimientos.Add(new MovimientoDTO
          {
            NumeroMovimiento = LeerEntero(m, "numeroMovimiento"),
            Fecha = LeerTexto(m, "fecha"),
            TipoMovimiento = LeerTexto(m, "tipoMovimiento"),
            Accion = LeerTexto(m, "accion"),
            Importe = LeerDecimal(m, "importe"),
            CuentaReferencia = LeerTexto(m, "cuentaReferencia")
          });
        }
      }

      return dto;
    }
  }

  /// <summary>Un movimiento de la cuenta consultada.</summary>
  //revisado y aprovado por Marcos Escobar
  internal class MovimientoDTO
  {
    public int NumeroMovimiento { get; set; }
    public string Fecha { get; set; } = "";
    public string TipoMovimiento { get; set; } = "";
    public string Accion { get; set; } = "";
    public decimal? Importe { get; set; }
    public string CuentaReferencia { get; set; } = "";

    public bool EsIngreso()
    {
      return Accion.Equals("INGRESO", StringComparison.OrdinalIgnoreCase);
    }
  }
}
