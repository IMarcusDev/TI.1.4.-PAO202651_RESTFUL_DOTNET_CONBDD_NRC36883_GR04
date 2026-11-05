using System.Text.Json;
using System.Text.Json.Nodes;

namespace ec.edu.monster.modelos.rest;

/// <summary>
/// Reads the JSON response by field name, ignoring case and nesting level, and whether estado
/// arrives as text ("OK") or as the .NET enum number (0 = OK, 1 = ERROR), as the server sends it.
/// </summary>
public class RestLectorRespuesta
{
    public const string EstadoOk = "OK";
    public const string EstadoError = "ERROR";

    private readonly JsonObject _raiz;

    public RestLectorRespuesta(RespuestaHttp respuesta, string url)
    {
        _raiz = Parsear(respuesta, url);

        // A 404 or 500 without the structured response (e.g. the server error page).
        if (!respuesta.EsExitosa && Valor("estado") is null)
            throw new HttpEstadoException(respuesta.Codigo, url);
    }

    public string? Estado()
    {
        JsonValue? valor = Valor("estado");
        if (valor is null)
            return null;
        if (valor.GetValueKind() == JsonValueKind.Number)
        {
            return valor.TryGetValue(out int numero)
                ? numero switch { 0 => EstadoOk, 1 => EstadoError, _ => numero.ToString() }
                : valor.ToJsonString();
        }
        return ComoTexto(valor)?.ToUpperInvariant();
    }

    public bool EsOk() => Estado() == EstadoOk;

    public string? Texto(string nombre) => ComoTexto(Valor(nombre));

    /// <summary>
    /// Returns, as name-value dictionaries, the objects that contain the given field. This reads
    /// lists (e.g. the movements) without depending on the name of the array.
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, string>> Registros(string campoClave)
    {
        var registros = new List<IReadOnlyDictionary<string, string>>();
        foreach (JsonObject objeto in Objetos().Where(o => Clave(o, campoClave) is not null))
        {
            var registro = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var (nombre, valor) in objeto)
            {
                if (valor is null or JsonValue)
                    registro[nombre] = ComoTexto(valor as JsonValue) ?? "";
            }
            registros.Add(registro);
        }
        return registros;
    }

    /// <summary>First simple value with that name, searching level by level from the root.</summary>
    private JsonValue? Valor(string nombre)
    {
        foreach (JsonObject objeto in Objetos())
        {
            string? clave = Clave(objeto, nombre);
            if (clave is not null && objeto[clave] is JsonValue valor)
                return valor;
        }
        return null;
    }

    /// <summary>Every object of the document, breadth first, keeping the order of the arrays.</summary>
    private IEnumerable<JsonObject> Objetos()
    {
        var pendientes = new Queue<JsonNode>();
        pendientes.Enqueue(_raiz);
        while (pendientes.Count > 0)
        {
            JsonNode actual = pendientes.Dequeue();
            IEnumerable<JsonNode?> hijos = actual switch
            {
                JsonObject objeto => objeto.Select(p => p.Value),
                JsonArray arreglo => arreglo,
                _ => [],
            };
            if (actual is JsonObject actualObjeto)
                yield return actualObjeto;
            foreach (JsonNode? hijo in hijos)
            {
                if (hijo is JsonObject or JsonArray)
                    pendientes.Enqueue(hijo);
            }
        }
    }

    private static string? Clave(JsonObject objeto, string nombre)
        => objeto.ContainsKey(nombre)
            ? nombre
            : objeto.Select(p => p.Key).FirstOrDefault(k => string.Equals(k, nombre, StringComparison.OrdinalIgnoreCase));

    private static string? ComoTexto(JsonValue? valor) => valor?.GetValueKind() switch
    {
        null or JsonValueKind.Null => null,
        JsonValueKind.String => valor.GetValue<string>().Trim(),
        // Numbers and booleans as they come in the JSON (379.50 keeps its decimals).
        _ => valor.ToJsonString(),
    };

    private static JsonObject Parsear(RespuestaHttp respuesta, string url)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(respuesta.Cuerpo))
                throw new RespuestaInvalidaException("Respuesta vacía");
            return JsonNode.Parse(respuesta.Cuerpo) as JsonObject
                   ?? throw new RespuestaInvalidaException("La respuesta no es un objeto JSON");
        }
        catch (Exception e) when (e is JsonException or RespuestaInvalidaException)
        {
            if (!respuesta.EsExitosa)
                throw new HttpEstadoException(respuesta.Codigo, url);
            throw e as RespuestaInvalidaException ?? new RespuestaInvalidaException("La respuesta no es un JSON válido", e);
        }
    }
}
