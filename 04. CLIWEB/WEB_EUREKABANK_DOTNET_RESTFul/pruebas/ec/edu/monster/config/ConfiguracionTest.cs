using ec.edu.monster.fakes;
using ec.edu.monster.modelos.servicios;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ec.edu.monster.config;

public class ConfiguracionTest
{
    [Fact]
    public void UrlBaseUsesHostAndPuerto()
        => Assert.Equal("http://192.168.1.20:5088/Login",
            new ServidorOptions { Host = " 192.168.1.20 ", Puerto = 5088 }.Url(RutasApi.RecursoLogin));

    [Fact]
    public void TheAppReadsTheServerFromAppsettingsJson()
    {
        using var app = new WebApplicationFactory<Program>();
        using IServiceScope alcance = app.Services.CreateScope();

        Assert.Equal("http://localhost:5088/Login", alcance.ServiceProvider.GetRequiredService<LoginService>().Url);
        Assert.Equal("http://localhost:5088/Movimiento", alcance.ServiceProvider.GetRequiredService<MovimientoService>().Url);
    }

    [Fact]
    public async Task ChangingHostAndPuertoMovesEveryService()
    {
        using var app = new AppPrueba();
        using (IServiceScope alcance = app.Services.CreateScope())
            Assert.Equal($"http://{AppPrueba.Host}:{AppPrueba.Puerto}/Movimiento",
                alcance.ServiceProvider.GetRequiredService<MovimientoService>().Url);

        // A real request goes to the configured remote server, not to localhost
        Navegador navegador = app.Navegador();
        await navegador.LoginEmpleadoAsync();
        Assert.Equal($"http://{AppPrueba.Host}:{AppPrueba.Puerto}/Login/loginEmpleado", app.Transporte.Ultima("loginEmpleado").Url);
    }
}
