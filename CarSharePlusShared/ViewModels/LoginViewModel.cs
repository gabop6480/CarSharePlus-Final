using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarSharePlusShared.Services;
using CarSharePlusShared.Models;
using Microsoft.Maui.Controls; // <--- CRUCIAL PARA SHELL

namespace CarSharePlusShared.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string correo;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string mensajeError;

        [ObservableProperty]
        private bool isBusy;

        public LoginViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                MensajeError = string.Empty;

                if (string.IsNullOrWhiteSpace(Correo) || string.IsNullOrWhiteSpace(Password))
                {
                    MensajeError = "Ingrese sus credenciales.";
                    return;
                }

                var usuario = await _authService.LoginAsync(Correo, Password);

                if (usuario != null)
                {
                    // Navegación absoluta al Dashboard (borra el historial de login)
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                else
                {
                    MensajeError = "Correo o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}