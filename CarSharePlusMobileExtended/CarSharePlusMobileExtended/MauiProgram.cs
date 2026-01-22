using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CarSharePlusShared.Services;
using CarSharePlusShared.ViewModels;
using CarSharePlusMobileExtended.Pages;
using CarSharePlusMobileExtended.ViewModels;
using Microcharts.Maui;
using System.Net.Http; // Necesario para HttpClient

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

            // 1. CLIENTE HTTP COMPARTIDO (Mantiene la sesión activa)
            builder.Services.AddSingleton<HttpClient>(sp =>
            {
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    CookieContainer = new System.Net.CookieContainer(),
                    UseProxy = false // Importante para emuladores
                };

                // Detecta si es Android (Emulador) o Windows
                string urlBase = DeviceInfo.Platform == DevicePlatform.Android
                    ? "http://10.0.2.2:5136"
                    : "http://localhost:5136";

                return new HttpClient(handler) { BaseAddress = new Uri(urlBase) };
            });

            // 2. SERVICIOS (Usan el cliente de arriba)
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<VehiculoService>();
            builder.Services.AddSingleton<UsuarioService>();
            builder.Services.AddSingleton<PagoService>();
            builder.Services.AddSingleton<DashboardService>();

            // 3. VIEWMODELS
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<VehiculosViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<PagosViewModel>();

            // 4. PÁGINAS
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ReservarVehiculoPage>();
            builder.Services.AddTransient<PerfilPage>();
            builder.Services.AddTransient<PagosPage>();

            return builder.Build();
        }
    }
}