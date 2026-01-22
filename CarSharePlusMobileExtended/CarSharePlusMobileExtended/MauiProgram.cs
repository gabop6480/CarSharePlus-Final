using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CarSharePlusShared.Services;
using CarSharePlusShared.ViewModels;
using CarSharePlusMobileExtended.Pages;
using CarSharePlusMobileExtended.ViewModels; // IMPORTANTE: Aquí está tu DashboardViewModel real

namespace CarSharePlusMobileExtended
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registrar el servicio (sin parámetros extra, porque ya los maneja internamente)
            builder.Services.AddSingleton<CarSharePlusShared.Services.AuthService>();

            // Registrar ViewModels y Pages
            builder.Services.AddTransient<CarSharePlusShared.ViewModels.LoginViewModel>();
            builder.Services.AddTransient<CarSharePlusMobileExtended.Pages.LoginPage>();
            builder.Services.AddTransient<CarSharePlusMobileExtended.ViewModels.DashboardViewModel>();
            builder.Services.AddTransient<CarSharePlusMobileExtended.Pages.DashboardPage>();

            return builder.Build();
        }
    }
}