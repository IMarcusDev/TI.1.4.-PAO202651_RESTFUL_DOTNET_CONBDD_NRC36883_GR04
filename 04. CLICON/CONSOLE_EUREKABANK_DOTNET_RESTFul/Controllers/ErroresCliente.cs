using System.Text.Json;

namespace CONSOLE_EUREKABANK_DOTNET_RESTFul.Controllers
{
  /// <summary>Mensajes del cliente cuando no hay respuesta válida del servidor (ECUD1-ECUD4), iguales a los del cliente móvil.</summary>
  //revisado y aprovado por Marcos Escobar
  internal static class ErroresCliente
  {
    public const string SinServidor = "No se pudo conectar con el servidor. Verifique la red e intente nuevamente.";
    public const string RespuestaIncompleta = "El servidor envió una respuesta vacía o incompleta.";
    public const string SesionExpirada = "Su sesión expiró o no es válida. Inicie sesión nuevamente.";

    /// <summary>Sin conexión o sin respuesta en 15 segundos, o una respuesta que no se puede leer.</summary>
    public static string Mensaje(Exception e)
    {
      return e is JsonException or KeyNotFoundException or InvalidOperationException or ArgumentException
        ? RespuestaIncompleta
        : SinServidor;
    }
  }
}
