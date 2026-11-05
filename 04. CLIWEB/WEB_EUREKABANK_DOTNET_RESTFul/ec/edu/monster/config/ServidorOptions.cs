namespace ec.edu.monster.config;

/// <summary>
/// The only place that knows where the REST server is: section "Servidor" of appsettings.json
/// (Host and Puerto). Every service receives it by injection and builds its URL from UrlBase.
/// </summary>
public class ServidorOptions
{
    public const string Seccion = "Servidor";

    public string Host { get; set; } = "localhost";

    public int Puerto { get; set; } = 5088;

    public string UrlBase => $"http://{Host.Trim()}:{Puerto}/";

    /// <summary>Full address of a resource, e.g. http://localhost:5088/Login.</summary>
    public string Url(string recurso) => UrlBase + recurso;
}
