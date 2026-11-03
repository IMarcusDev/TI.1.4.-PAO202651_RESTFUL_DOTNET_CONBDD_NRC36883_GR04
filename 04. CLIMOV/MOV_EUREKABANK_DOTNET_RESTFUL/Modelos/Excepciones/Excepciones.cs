namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Excepciones;

/// <summary>HTTP 4xx/5xx sin la respuesta estructurada (estado y mensaje).</summary>
public class HttpEstadoException(int codigo, string url) : IOException($"HTTP {codigo} en {url}")
{
    public int Codigo { get; } = codigo;
}

public class RespuestaInvalidaException(string mensaje, Exception? causa = null) : Exception(mensaje, causa);
