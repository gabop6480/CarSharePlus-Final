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

        public VehiculosViewModel(VehiculoService vehiculoService)
        {
            _vehiculoService = vehiculoService;
            ListaVehiculos = new ObservableCollection<Vehiculo>();
            // Llamamos a cargar vehículos de forma segura
            CargarVehiculosCommand.Execute(null);
        }

        [RelayCommand]
        private async Task CargarVehiculos()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 1. Descarga los datos (esto ocurre en segundo plano)
                var vehiculos = await _vehiculoService.GetVehiculosAsync();

                // 2. CORRECCIÓN CRÍTICA PARA WINDOWS:
                // Usar el Hilo Principal para actualizar la lista visual
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ListaVehiculos.Clear();
                    foreach (var v in vehiculos)
                    {
                        ListaVehiculos.Add(v);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando vehículos: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}