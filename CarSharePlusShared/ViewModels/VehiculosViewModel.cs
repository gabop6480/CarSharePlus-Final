using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CarSharePlusShared.Models;
using CarSharePlusShared.Services;

namespace CarSharePlusShared.ViewModels
{
    public partial class VehiculosViewModel : ObservableObject
    {
        private readonly VehiculoService _vehiculoService;

        [ObservableProperty]
        private ObservableCollection<Vehiculo> listaVehiculos;

        [ObservableProperty]
        private bool isBusy;

        // Inyectamos el servicio real
        public VehiculosViewModel(VehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
            ListaVehiculos = new ObservableCollection<Vehiculo>();
            Task.Run(CargarVehiculos); // Cargar al iniciar
        }

        [RelayCommand]
        private async Task CargarVehiculos()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                var vehiculos = await _vehiculoService.GetVehiculosAsync();
                ListaVehiculos.Clear();
                foreach (var v in vehiculos)
                {
                    ListaVehiculos.Add(v);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}