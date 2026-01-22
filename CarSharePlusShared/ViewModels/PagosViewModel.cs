using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CarSharePlusShared.Models;
using CarSharePlusShared.Services;

namespace CarSharePlusShared.ViewModels
{
    public partial class PagosViewModel : ObservableObject
    {
        private readonly PagoService _pagoService;

        [ObservableProperty] private ObservableCollection<Pago> listaPagos;
        [ObservableProperty] private bool isBusy;

        public PagosViewModel(PagoService pagoService)
        {
            _pagoService = pagoService;
            ListaPagos = new ObservableCollection<Pago>();
            Task.Run(CargarPagos);
        }

        [RelayCommand]
        public async Task CargarPagos()
        {
            if (AuthService.UsuarioActual == null) return;

            IsBusy = true;
            var pagos = await _pagoService.GetPagosByUsuarioAsync(AuthService.UsuarioActual.Id);

            ListaPagos.Clear();
            foreach (var p in pagos) ListaPagos.Add(p);

            IsBusy = false;
        }
    }
}