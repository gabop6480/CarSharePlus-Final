using CarSharePlusShared.Models;
using CarSharePlusShared.Services; // Necesario para UsuarioService
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace CarSharePlusShared.ViewModels
{
    public partial class PerfilViewModel : ObservableObject
    {
        private readonly UsuarioService _usuarioService;
        private readonly AuthService _authService; // Para poder cerrar sesión

        [ObservableProperty]
        private Usuario? usuario;

        public PerfilViewModel(UsuarioService usuarioService, AuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;

            // Cargar perfil real al iniciar
            Task.Run(CargarPerfil);
        }

        [RelayCommand]
        public async Task CargarPerfil()
        {
            var perfil = await _usuarioService.GetPerfilAsync();
            if (perfil != null)
            {
                Usuario = perfil;
            }
        }

        [RelayCommand]
        public async Task GuardarCambios()
        {
            if (Usuario != null)
            {
                bool exito = await _usuarioService.ActualizarPerfilAsync(Usuario);
                if (exito)
                {
                    await CargarPerfil(); // Recargar para confirmar cambios
                }
            }
        }

        [RelayCommand]
        public async Task CerrarSesion()
        {
            await _authService.LogoutAsync();
            // La navegación al Login la maneja la UI (AppShell)
        }
    }
}