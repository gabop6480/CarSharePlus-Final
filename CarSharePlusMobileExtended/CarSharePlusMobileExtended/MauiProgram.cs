using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting; // Importante para gráficas
using CarSharePlusMobileExtended.Pages;
using CarSharePlusMobileExtended.ViewModels;
using CarSharePlusShared.Services;
using CarSharePlusShared.ViewModels;
using System.Net;

namespace CarSharePlusMobileExtended
{
    public static class MauiProgram
    {
        #if ANDROID
        const string BaseUrl = "http://10.0.2.2:5136";
#else
        const string BaseUrl = "http://localhost:5136";
#endif

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if ANDROID || IOS
            builder.UseMauiMaps();
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // 1. CONFIGURACIÓN HTTP
            builder.Services.AddSingleton(sp =>
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                };
                return new HttpClient(handler);
            });

            // 2. SERVICIOS (¡Faltaban estos en tu archivo!)
            builder.Services.AddSingleton(sp => new AuthService(sp.GetRequiredService<HttpClient>(), BaseUrl));
            builder.Services.AddSingleton<IReservaService>(sp => new ReservaService(sp.GetRequiredService<HttpClient>(), BaseUrl));
            builder.Services.AddSingleton(sp => new DashboardService(sp.GetRequiredService<HttpClient>(), BaseUrl));
            builder.Services.AddSingleton(sp => new PagoService(sp.GetRequiredService<HttpClient>(), BaseUrl + "/api/pagos"));
            builder.Services.AddSingleton(sp => new EvaluacionService(sp.GetRequiredService<HttpClient>(), BaseUrl));
            builder.Services.AddSingleton(sp => new UsuarioService(sp.GetRequiredService<HttpClient>(), BaseUrl));
            // Servicio de mapas
            builder.Services.AddSingleton(sp => new OverpassService(sp.GetRequiredService<HttpClient>()));

            // 3. VIEWMODELS (Registrar TODOS para evitar caídas)
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<EvaluacionesViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<MapasViewModel>();
            builder.Services.AddTransient<AgregarEvaluacionViewModel>();
            builder.Services.AddTransient<EditarEvaluacionViewModel>();
            builder.Services.AddTransient<PagosViewModel>();
            builder.Services.AddTransient<ReservasViewModel>();
            builder.Services.AddTransient<ReservaViewModel>();
            builder.Services.AddTransient<VehiculoViewModel>();

            // 4. PAGES
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<MapasPage>();
            builder.Services.AddTransient<EvaluacionesPage>();
            builder.Services.AddTransient<AgregarEvaluacionPage>();
            builder.Services.AddTransient<EditarEvaluacionPage>();
            builder.Services.AddTransient<PagosPage>();
            builder.Services.AddTransient<PerfilPage>();
            builder.Services.AddTransient<ReservarVehiculoPage>();
            builder.Services.AddTransient<ReservasPage>();
            builder.Services.AddTransient<RecomendacionesPage>();

            return builder.Build();
        }
    }
}