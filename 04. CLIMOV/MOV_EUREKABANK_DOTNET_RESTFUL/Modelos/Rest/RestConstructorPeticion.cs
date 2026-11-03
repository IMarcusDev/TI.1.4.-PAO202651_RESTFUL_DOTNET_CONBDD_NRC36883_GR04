using System.Text.Json.Nodes;

namespace MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;

/// <summary>Cuerpo JSON de una petición: un objeto plano con los parámetros de la operación.</summary>
public class RestConstructorPeticion(string ruta)
{
    private readonly JsonObject _cuerpo = [];

    public string Ruta { get; } = ruta;

    public RestConstructorPeticion Parametro(string nombre, string? valor)
    {
        if (valor is not null)
            _cuerpo[nombre] = valor;
        return this;
    }

    /// <summary>El importe viaja como número JSON con dos decimales (120.50).</summary>
    public RestConstructorPeticion Parametro(string nombre, decimal valor)
    {
        _cuerpo[nombre] = decimal.Round(valor, 2);
        return this;
    }

    public string Construir() => _cuerpo.ToJsonString();
}
