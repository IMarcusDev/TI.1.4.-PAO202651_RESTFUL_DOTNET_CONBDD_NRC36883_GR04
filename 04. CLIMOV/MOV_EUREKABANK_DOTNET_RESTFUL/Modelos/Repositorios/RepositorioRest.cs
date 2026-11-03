using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Data;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Interfaces;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;
using System.Globalization;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Repositorios;

/// <summary>Base de los repositorios REST: envía la petición y traduce la respuesta estructurada del servidor.</summary>
public abstract class RepositorioRest
{
    private readonly string _urlRecurso;
    private readonly RestTransporte _transporte;
    private readonly IVerificadorRed _red;

    protected RepositorioRest(string urlBase, string recurso, RestTransporte transporte, IVerificadorRed red)
    {
        _urlRecurso = (urlBase.EndsWith('/') ? urlBase : urlBase + "/") + recurso + "/";
        _transporte = transporte;
        _red = red;
    }

    protected static RestConstructorPeticion Peticion(string operacion) => new(operacion);

    protected bool SinRed() => _red.SinRed();

    protected async Task<RestLectorRespuesta> EnviarAsync(RestConstructorPeticion peticion)
    {
        string url = _urlRecurso + peticion.Ruta;
        return new RestLectorRespuesta(await _transporte.EnviarAsync(url, peticion.Construir()), url);
    }

    /// <summary>Convierte una respuesta con estado ERROR en un resultado, distinguiendo el token inválido.</summary>
    protected static Resultado<T> Fallo<T>(RestLectorRespuesta respuesta, string porDefecto)
    {
        string mensaje = RestErrores.MensajeServidor(respuesta.Texto("mensaje"), porDefecto);
        return RestErrores.EsTokenInvalido(mensaje)
            ? Resultado<T>.Expirada(RestErrores.SesionExpirada)
            : Resultado<T>.Error(mensaje);
    }

    protected static decimal? Decimal(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return null;
        return decimal.TryParse(texto.Trim().Replace(',', '.'), NumberStyles.Number | NumberStyles.AllowExponent,
                                CultureInfo.InvariantCulture, out decimal valor)
            ? valor
            : null;
    }

    protected static string TextoOVacio(string? texto) => texto ?? "";
}
