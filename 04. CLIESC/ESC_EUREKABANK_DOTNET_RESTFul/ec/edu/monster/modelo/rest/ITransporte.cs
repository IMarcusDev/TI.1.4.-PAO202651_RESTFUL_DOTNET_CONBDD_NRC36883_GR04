namespace ec.edu.monster.modelo.rest;

/// <summary>HTTP status and body of a response.</summary>
public record RespuestaHttp(int Codigo, string Cuerpo)
{
    public bool EsExitosa => Codigo is >= 200 and <= 299;
}

/// <summary>Sends one POST with a JSON body and returns the response, whatever its status.</summary>
public interface ITransporte
{
    /// <param name="url">Full operation address, e.g. http://localhost:5088/Login/loginEmpleado.</param>
    /// <param name="json">Request body.</param>
    Task<RespuestaHttp> EnviarAsync(string url, string json);
}
