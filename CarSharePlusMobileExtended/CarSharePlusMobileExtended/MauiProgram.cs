using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CarSharePlusShared.Services;
using CarSharePlusShared.ViewModels;
using CarSharePlusMobileExtended.Pages;
using CarSharePlusMobileExtended.ViewModels;
using Microcharts.Maui;
using System.Net.Http; // <--- NECESARIO

namespace CarSharePlusMobileExtended
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // --- 1. CONFIGURACIÓN DE CONEXIÓN (Vital para Login y Datos) ---
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new System.Net.CookieContainer(),
                    UseProxy = false
                };

                // URL para Windows (localhost) y Android (10.0.2.2)
                string urlBase = DeviceInfo.Platform == DevicePlatform.Android
                    ? "http://10.0.2.2:5136"
                    : "http://localhost:5136";

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(urlBase),
                    Timeout = TimeSpan.FromSeconds(30)
                };
            });

            // --- 2. SERVICIOS (Usan la conexión de arriba) ---
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<VehiculoService>();
            builder.Services.AddSingleton<UsuarioService>();
            builder.Services.AddSingleton<PagoService>();
            builder.Services.AddSingleton<DashboardService>();

            // --- 3. VIEWMODELS ---
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<VehiculosViewModel>(); // Plural
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<PagosViewModel>();

            // --- 4. PÁGINAS ---
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ReservarVehiculoPage>();
            builder.Services.AddTransient<PagosPage>();
            builder.Services.AddTransient<PerfilPage>();

            return builder.Build();
        }
    }
}