using System.IO;

namespace ec.edu.monster.modelo.rest;

/// <summary>The server answered with an HTTP error status that carries no estado/mensaje body.</summary>
public class HttpEstadoException(int codigo, string url) : IOException($"HTTP {codigo} en {url}")
{
    public int Codigo { get; } = codigo;
}

/// <summary>The response is empty or is not a JSON object.</summary>
public class RespuestaInvalidaException(string mensaje, Exception? causa = null) : Exception(mensaje, causa);
