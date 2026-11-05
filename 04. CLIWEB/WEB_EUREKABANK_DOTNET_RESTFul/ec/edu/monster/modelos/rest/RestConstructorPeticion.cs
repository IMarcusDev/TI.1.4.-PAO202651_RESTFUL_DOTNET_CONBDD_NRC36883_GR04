using System.Text.Json.Nodes;

namespace ec.edu.monster.modelos.rest;

/// <summary>JSON body of a request: a flat object with the operation parameters (camelCase).</summary>
public class RestConstructorPeticion(string operacion)
{
    private readonly JsonObject _cuerpo = [];

    /// <summary>Last segment of the route, e.g. "loginEmpleado" in Login/loginEmpleado.</summary>
    public string Operacion { get; } = operacion;

    public RestConstructorPeticion Parametro(string nombre, string? valor)
    {
        if (valor is not null)
            _cuerpo[nombre] = valor;
        return this;
    }

    /// <summary>The amount travels as a JSON number with two decimals (120.50).</summary>
    public RestConstructorPeticion Parametro(string nombre, decimal valor)
    {
        // Adding 0.00m keeps two decimal places in the serialized number (120.5 -> 120.50).
        _cuerpo[nombre] = decimal.Round(valor, 2) + 0.00m;
        return this;
    }

    public string Construir() => _cuerpo.ToJsonString();
}
