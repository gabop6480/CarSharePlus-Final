using CommunityToolkit.Mvvm.ComponentModel;
using CarSharePlusShared.Models;
using CarSharePlusShared.Services;

namespace CarSharePlusShared.ViewModels
{
    public partial class PerfilViewModel : ObservableObject
    {
        [ObservableProperty] private Usuario? usuario;

        public PerfilViewModel()
        {
            // Cargamos el usuario que guardamos en el Login
            Usuario = AuthService.UsuarioActual;
        }
    }
}