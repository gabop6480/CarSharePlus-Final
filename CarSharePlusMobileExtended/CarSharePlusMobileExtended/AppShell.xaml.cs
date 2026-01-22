using CarSharePlusShared.Services;

namespace CarSharePlusMobileExtended
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                // 1. Obtener el servicio de autenticación
                var authService = Handler?.MauiContext?.Services.GetService<AuthService>();

                if (authService != null)
                {
                    await authService.LogoutAsync();
                }

                // 2. Navegar de vuelta al Login (usando /// para limpiar el historial)
                await Current.GoToAsync("//LoginPage");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cerrar sesión: {ex.Message}");
                // En caso de fallo, forzamos la navegación
                await Current.GoToAsync("//LoginPage");
            }
        }
    }
}