using Microsoft.Extensions.Logging;
using MOV_EUREKABANK_DOTNET_RESTFUL.Config;
using MOV_EUREKABANK_DOTNET_RESTFUL.Controlador;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Interfaces;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Repositorios;
using MOV_EUREKABANK_DOTNET_RESTFUL.Modelos.Rest;
using MOV_EUREKABANK_DOTNET_RESTFUL.Servicios;
using MOV_EUREKABANK_DOTNET_RESTFUL.Vista;

namespace MOV_EUREKABANK_DOTNET_RESTFUL
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Modelo: la sesión y los repositorios REST viven mientras dura la app.
            builder.Services.AddSingleton<SesionServicio>();
            builder.Services.AddSingleton<IVerificadorRed, RedMaui>();
            builder.Services.AddSingleton<RestTransporte>();
            builder.Services.AddSingleton<ILoginRepositorio>(s =>
                new LoginRestRepositorio(RestConfig.UrlBase, s.GetRequiredService<RestTransporte>(), s.GetRequiredService<IVerificadorRed>()));
            builder.Services.AddSingleton<IMovimientoRepositorio>(s =>
                new MovimientoRestRepositorio(RestConfig.UrlBase, s.GetRequiredService<RestTransporte>(), s.GetRequiredService<IVerificadorRed>()));

            // Controladores y vistas
            builder.Services.AddTransient<LoginControlador>();
            builder.Services.AddTransient<MovimientosControlador>();
            builder.Services.AddTransient<OperacionControlador>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<InicioPage>();
            builder.Services.AddTransient<MovimientosPage>();
            builder.Services.AddTransient<OperacionesPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Sin la línea inferior que Android dibuja en los Entry: el contorno lo pone CampoTexto.
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SinSubrayado", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            return builder.Build();
        }
    }
}
