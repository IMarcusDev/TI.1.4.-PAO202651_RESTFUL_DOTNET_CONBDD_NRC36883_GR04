using MOV_EUREKABANK_DOTNET_RESTFUL.Config;
using System.Text;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;

public record RespuestaHttp(int Codigo, string Cuerpo)
{
    public bool EsExitosa => Codigo is >= 200 and <= 299;
}

public class RestTransporte
{
    private readonly HttpClient _http;

    public RestTransporte() : this(RestConfig.TiempoEspera)
    {
    }

    public RestTransporte(TimeSpan tiempoEspera)
    {
        _http = new HttpClient { Timeout = tiempoEspera };
    }

    /// <summary>Envía un POST con cuerpo JSON. Devuelve también las respuestas 4xx/5xx, que pueden traer estado y mensaje.</summary>
    public virtual async Task<RespuestaHttp> EnviarAsync(string url, string json)
    {
        using var peticion = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json"),
        };
        peticion.Headers.Accept.ParseAdd("application/json");

        using HttpResponseMessage respuesta = await _http.SendAsync(peticion);
        return new RespuestaHttp((int)respuesta.StatusCode, await respuesta.Content.ReadAsStringAsync());
    }
}
