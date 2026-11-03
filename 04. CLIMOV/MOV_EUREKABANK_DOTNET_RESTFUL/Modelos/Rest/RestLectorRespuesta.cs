using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Excepciones;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;

/// <summary>
/// Lee el JSON de la respuesta por nombre de campo, sin importar mayúsculas, el nivel de anidamiento
/// ni si el estado llega como texto ("OK") o como número (0 = OK, 1 = ERROR, el enum de .NET sin convertidor).
/// </summary>
public class RestLectorRespuesta
{
    public const string EstadoOk = "OK";
    public const string EstadoError = "ERROR";

    private readonly JsonObject _raiz;

    public RestLectorRespuesta(RespuestaHttp respuesta, string url)
    {
        _raiz = Parsear(respuesta, url);

        // Un 404 o un 500 sin la respuesta estructurada (por ejemplo, la página de error del servidor).
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
            int numero = valor.GetValue<int>();
            return numero switch { 0 => EstadoOk, 1 => EstadoError, _ => numero.ToString() };
        }
        return ComoTexto(valor)?.ToUpperInvariant();
    }

    public bool EsOk() => Estado() == EstadoOk;

    public string? Texto(string nombre) => ComoTexto(Valor(nombre));

    /// <summary>
    /// Devuelve, como diccionarios nombre-valor, los objetos que contienen el campo indicado.
    /// Así se leen las listas (por ejemplo, los movimientos) sin depender de cómo se llame el arreglo.
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

    /// <summary>Primer valor simple con ese nombre, buscando nivel por nivel desde la raíz.</summary>
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

    /// <summary>Todos los objetos del documento en orden de anchura, respetando el orden de los arreglos.</summary>
    private IEnumerable<JsonObject> Objetos()
    {
        var pendientes = new Queue<JsonNode>();
        pendientes.Enqueue(_raiz);
        while (pendientes.Count > 0)
        {
            JsonNode actual = pendientes.Dequeue();
            if (actual is JsonObject objeto)
            {
                yield return objeto;
                foreach (var (_, hijo) in objeto)
                {
                    if (hijo is JsonObject or JsonArray)
                        pendientes.Enqueue(hijo);
                }
            }
            else if (actual is JsonArray arreglo)
            {
                foreach (JsonNode? hijo in arreglo)
                {
                    if (hijo is JsonObject or JsonArray)
                        pendientes.Enqueue(hijo);
                }
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
        // Números y booleanos tal como vienen en el JSON (379.50 conserva sus decimales).
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
