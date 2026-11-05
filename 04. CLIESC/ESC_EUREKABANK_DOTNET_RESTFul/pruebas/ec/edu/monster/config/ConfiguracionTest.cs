using System.IO;
using ec.edu.monster.config;

namespace ec.edu.monster.config;

public class ConfiguracionTest
{
    [Fact]
    public void UrlBaseUsesHostAndPuerto()
        => Assert.Equal("http://192.168.1.20:5088/Login",
            new Configuracion(" 192.168.1.20 ", 5088).Url(RutasApi.RecursoLogin));

    [Fact]
    public void TheDefaultsPointAtLocalhostAndPort5088()
    {
        var configuracion = new Configuracion();

        Assert.Equal("localhost", configuracion.Host);
        Assert.Equal(5088, configuracion.Puerto);
        Assert.Equal("http://localhost:5088/", configuracion.UrlBase);
    }

    [Fact]
    public void ReadsTheServidorSectionFromTheFile()
    {
        string archivo = ArchivoTemporal("{\"Servidor\":{\"Host\":\"10.0.0.5\",\"Puerto\":5180}}");
        try
        {
            Configuracion configuracion = Configuracion.Cargar(archivo);

            Assert.Equal("10.0.0.5", configuracion.Host);
            Assert.Equal(5180, configuracion.Puerto);
            Assert.Equal("http://10.0.0.5:5180/Movimiento",
                configuracion.Url(RutasApi.RecursoMovimiento));
        }
        finally
        {
            File.Delete(archivo);
        }
    }

    [Fact]
    public void AMissingFileFallsBackToTheDefaults()
        => Assert.Equal("http://localhost:5088/Login",
            Configuracion.Cargar(Path.Combine(Path.GetTempPath(), "eurekabank-no-existe.json"))
                .Url(RutasApi.RecursoLogin));

    [Fact]
    public void AFileWithoutTheServidorSectionFallsBackToTheDefaults()
    {
        string archivo = ArchivoTemporal("{\"Otro\":true}");
        try
        {
            Assert.Equal(5088, Configuracion.Cargar(archivo).Puerto);
        }
        finally
        {
            File.Delete(archivo);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(70000)]
    public void RejectsAnOutOfRangePort(int puerto)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Configuracion("localhost", puerto));

    private static string ArchivoTemporal(string contenido)
    {
        string archivo = Path.Combine(Path.GetTempPath(), $"eurekabank-{Guid.NewGuid():N}.json");
        File.WriteAllText(archivo, contenido);
        return archivo;
    }
}
