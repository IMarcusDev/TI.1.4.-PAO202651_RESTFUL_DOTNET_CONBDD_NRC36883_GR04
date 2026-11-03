using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO
{
  internal class ResponseDTO
  {
    public EstadosEnum estado;
    public string mensaje;

    public ResponseDTO(EstadosEnum estado, string mensaje)
    {
      this.estado = estado;
      this.mensaje = mensaje;
    }

    public EstadosEnum GetEstado()
    {
      return estado;
    }

    public void SetEstado(EstadosEnum estado)
    {
      this.estado = estado;
    }

    public string GetMensaje()
    {
      return mensaje;
    }

    public void SetMensaje(string mensaje)
    {
      this.mensaje = mensaje;
    }

    //revisado y aprovado por Marcos Escobar
    // El servidor .NET envía el estado como número del enum (0 = OK, 1 = ERROR) y el Java como texto.
    public static ResponseDTO FromJSON(string json)
    {
      using JsonDocument doc = JsonDocument.Parse(json);

      JsonElement obj = doc.RootElement;
      JsonElement estadoElement = obj.GetProperty("estado");

      EstadosEnum estado = estadoElement.ValueKind == JsonValueKind.Number
        ? (EstadosEnum)estadoElement.GetInt32()
        : Enum.Parse<EstadosEnum>(estadoElement.GetString() ?? "ERROR", true);

      string mensaje = LeerTexto(obj, "mensaje");

      return new ResponseDTO(estado, mensaje);
    }

    //revisado y aprovado por Marcos Escobar
    // ECUD4: el servidor responde «Token inválido o sesión expirada»; el cliente vuelve al inicio de sesión.
    public bool SesionExpirada()
    {
      return estado == EstadosEnum.ERROR && (mensaje ?? "").Contains("Token");
    }

    //revisado y aprovado por Marcos Escobar
    public static string LeerTexto(JsonElement obj, string nombre)
    {
      if (!obj.TryGetProperty(nombre, out JsonElement valor))
        return "";

      return valor.ValueKind switch
      {
        JsonValueKind.String => valor.GetString() ?? "",
        JsonValueKind.Number => valor.GetRawText(),
        _ => ""
      };
    }

    //revisado y aprovado por Marcos Escobar
    // Los importes pueden llegar como número (120.5) o como texto ("120.50").
    public static decimal? LeerDecimal(JsonElement obj, string nombre)
    {
      string texto = LeerTexto(obj, nombre);

      return decimal.TryParse(
        texto,
        System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowExponent,
        System.Globalization.CultureInfo.InvariantCulture,
        out decimal valor
      ) ? valor : null;
    }

    //revisado y aprovado por Marcos Escobar
    public static int LeerEntero(JsonElement obj, string nombre)
    {
      return int.TryParse(LeerTexto(obj, nombre), out int valor) ? valor : 0;
    }
  }

}
