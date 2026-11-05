using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using ec.edu.monster.modelo.rest;

namespace ec.edu.monster.fakes;

/// <summary>
/// REST transport double: records every request and answers each operation (last route segment)
/// with a scripted JSON body (or exception). An operation without a script fails like an unreachable server.
/// </summary>
public class FakeTransporte : ITransporte
{
    private readonly Dictionary<string, Func<RespuestaHttp>> _respuestas = [];
    private readonly List<PeticionRest> _peticiones = [];

    public IReadOnlyList<PeticionRest> Peticiones
    {
        get { lock (_peticiones) return _peticiones.ToList(); }
    }

    /// <summary>Answers with HTTP 200, like WS_EUREKABANK_DOTNET_RESTFul does for OK and ERROR.</summary>
    public void Responder(string operacion, string json) => Responder(operacion, 200, json);

    public void Responder(string operacion, int codigo, string cuerpo)
        => _respuestas[operacion] = () => new RespuestaHttp(codigo, cuerpo);

    public void Fallar(string operacion, Exception error) => _respuestas[operacion] = () => throw error;

    public PeticionRest Ultima(string operacion) => Peticiones.Last(p => p.Operacion == operacion);

    public int Llamadas(string operacion) => Peticiones.Count(p => p.Operacion == operacion);

    public Task<RespuestaHttp> EnviarAsync(string url, string json)
    {
        string operacion = url[(url.LastIndexOf('/') + 1)..];
        lock (_peticiones) _peticiones.Add(new PeticionRest(url, json, operacion));

        if (!_respuestas.TryGetValue(operacion, out Func<RespuestaHttp>? respuesta))
            throw new HttpRequestException("Servidor no disponible en la prueba");
        return Task.FromResult(respuesta());
    }
}

/// <summary>One request sent through the fake transport.</summary>
public record PeticionRest(string Url, string Json, string Operacion)
{
    /// <summary>Value of a body field as text (numbers as written in the JSON), or null when it was not sent.</summary>
    public string? Parametro(string nombre)
    {
        JsonNode? valor = JsonNode.Parse(Json)?[nombre];
        if (valor is null)
            return null;
        return valor.GetValueKind() == JsonValueKind.String ? valor.GetValue<string>() : valor.ToJsonString();
    }

    /// <summary>Kind of a body field, to check that amounts travel as JSON numbers.</summary>
    public JsonValueKind Tipo(string nombre) => JsonNode.Parse(Json)?[nombre]?.GetValueKind() ?? JsonValueKind.Undefined;
}
