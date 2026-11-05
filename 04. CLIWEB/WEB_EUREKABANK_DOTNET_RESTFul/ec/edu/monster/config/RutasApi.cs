namespace ec.edu.monster.config;

/// <summary>
/// Fixed parts of the REST contract published by WS_EUREKABANK_DOTNET_RESTFul (no "api/" prefix).
/// They are not configuration: the user only changes Host and Puerto.
/// </summary>
public static class RutasApi
{
    public const string RecursoLogin = "Login";
    public const string RecursoMovimiento = "Movimiento";

    /// <summary>The client stops waiting after 15 seconds.</summary>
    public static readonly TimeSpan TiempoEspera = TimeSpan.FromSeconds(15);
}
