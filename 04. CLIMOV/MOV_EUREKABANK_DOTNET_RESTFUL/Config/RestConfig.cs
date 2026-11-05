namespace MOV_EUREKABANK_DOTNET_RESTFUL.Config;

public static class RestConfig
{
    /// <summary>
    /// Servidor REST de EurekaBank (WS_EUREKABANK_DOTNET_RESTFul, perfil http de launchSettings).
    /// 10.0.2.2 es el equipo anfitrión visto desde el emulador; en un celular físico use la IP del servidor.
    /// </summary>
    public const string UrlBase = "http://192.168.1.76:5088/";

    public const string RecursoLogin = "Login";
    public const string RecursoMovimiento = "Movimiento";

    /// <summary>RNF-007: el cliente deja de esperar a los 15 segundos.</summary>
    public const int TiempoEsperaSegundos = 15;
    public static readonly TimeSpan TiempoEspera = TimeSpan.FromSeconds(TiempoEsperaSegundos);
}
