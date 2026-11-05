using ec.edu.monster.config;
using ec.edu.monster.modelos.rest;
using Microsoft.Extensions.Options;

namespace ec.edu.monster.modelos.servicios;

/// <summary>Base of the REST services: sends the request and turns failures into messages.</summary>
public abstract class ServicioRest
{
    public const string RespuestaIncompleta = "El servidor envió una respuesta vacía o incompleta.";

    private readonly ITransporte _transporte;

    protected ServicioRest(ITransporte transporte, IOptions<ServidorOptions> opciones, string recurso)
    {
        _transporte = transporte;
        Url = opciones.Value.Url(recurso);
    }

    /// <summary>Address of the resource, also shown when the server cannot be reached.</summary>
    public string Url { get; }

    protected string SinConexion => "No se pudo conectar con el servidor: " + Url;

    protected static RestConstructorPeticion Peticion(string operacion) => new(operacion);

    protected async Task<RestLectorRespuesta> EnviarAsync(RestConstructorPeticion peticion)
    {
        string url = $"{Url}/{peticion.Operacion}";
        return new RestLectorRespuesta(await _transporte.EnviarAsync(url, peticion.Construir()), url);
    }

    /// <summary>User message for an exception thrown while calling the server.</summary>
    protected string Mensaje(Exception ex) => ex switch
    {
        HttpEstadoException http => FallaServidor(http.Codigo),
        RespuestaInvalidaException => RespuestaIncompleta,
        _ => SinConexion,
    };

    /// <summary>An HTTP 4xx/5xx without estado and mensaje (missing route or internal error).</summary>
    public static string FallaServidor(int codigo)
        => $"El servidor no pudo procesar la solicitud (HTTP {codigo}). Intente más tarde.";

    /// <summary>The server message, or the default when it sent none.</summary>
    protected static string MensajeServidor(string? mensaje, string porDefecto)
        => string.IsNullOrWhiteSpace(mensaje) ? porDefecto : mensaje.Trim();

    /// <summary>The server says "Token inválido o sesión expirada" when the token is no longer valid.</summary>
    public static bool EsTokenInvalido(string? mensaje)
        => mensaje is not null && mensaje.Contains("token", StringComparison.OrdinalIgnoreCase);
}
