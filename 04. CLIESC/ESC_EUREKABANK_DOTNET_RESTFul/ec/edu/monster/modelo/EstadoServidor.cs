using System.Net.Http;
using ec.edu.monster.config;

namespace ec.edu.monster.modelo;

/// <summary>
/// Asks the REST server whether it is up by requesting the Login resource. It asks on every call,
/// because the server may go down at any moment while the client is open.
/// </summary>
public class EstadoServidor(Configuracion configuracion, HttpClient http)
{
    /// <summary>A GET on the resource answers 405 (it only accepts POST); any answer proves the server is alive.</summary>
    public string Url { get; } = configuracion.Url(RutasApi.RecursoLogin);

    /// <summary>True when the server answers right now; any status below 500 means it is running.</summary>
    public async Task<bool> EstaDisponibleAsync()
    {
        try
        {
            using HttpResponseMessage respuesta = await http.GetAsync(Url);
            return (int)respuesta.StatusCode < 500;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
