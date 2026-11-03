using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Interfaces;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;
using System.Text.Json.Nodes;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Pruebas;

/// <summary>Transporte que no abre conexiones: registra cada petición y devuelve la respuesta indicada.</summary>
public class TransporteFalso(Func<string, RespuestaHttp> responder) : RestTransporte
{
    public record Peticion(string Url, string Json)
    {
        public JsonObject Cuerpo => JsonNode.Parse(Json)!.AsObject();
    }

    public List<Peticion> Peticiones { get; } = [];

    public Peticion Ultima => Peticiones[^1];

    public static TransporteFalso Responde(string json) => Responde(200, json);

    public static TransporteFalso Responde(int codigo, string cuerpo) => new(_ => new RespuestaHttp(codigo, cuerpo));

    public static TransporteFalso Lanza(Exception error) => new(_ => throw error);

    public override Task<RespuestaHttp> EnviarAsync(string url, string json)
    {
        Peticiones.Add(new Peticion(url, json));
        return Task.FromResult(responder(json));
    }

    public const string HtmlErrorKestrel = "<!DOCTYPE html><html><body><h1>404 Not Found</h1></body></html>";
}

public class RedFalsa(bool sinRed) : IVerificadorRed
{
    public bool SinRed() => sinRed;
}
