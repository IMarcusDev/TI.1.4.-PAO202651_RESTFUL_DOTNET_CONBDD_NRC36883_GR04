using System.Net.Http;
using System.Text;

namespace ec.edu.monster.modelo.rest;

/// <summary>REST over HTTP with HttpClient (the timeout is set where the client is registered).</summary>
public class TransporteRest(HttpClient http) : ITransporte
{
    /// <returns>Status and body; 4xx/5xx are returned too because they may carry estado and mensaje.</returns>
    public async Task<RespuestaHttp> EnviarAsync(string url, string json)
    {
        using var peticion = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
        peticion.Headers.Accept.ParseAdd("application/json");

        using HttpResponseMessage respuesta = await http.SendAsync(peticion);
        return new RespuestaHttp((int)respuesta.StatusCode, await respuesta.Content.ReadAsStringAsync());
    }
}
