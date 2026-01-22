using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CarSharePlusShared.Services;
using CarSharePlusShared.ViewModels;
using CarSharePlusMobileExtended.Pages;
using CarSharePlusMobileExtended.ViewModels;
using Microcharts.Maui; // <--- 1. IMPORTANTE: Agregar este using

namespace CarSharePlusMobileExtended
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts() // <--- 2. IMPORTANTE: Inicializar gráficos
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Servicios
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<DashboardService>(); // Asegúrate de registrar este servicio también

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<DashboardPage>();

            return builder.Build();
        }
    }
}