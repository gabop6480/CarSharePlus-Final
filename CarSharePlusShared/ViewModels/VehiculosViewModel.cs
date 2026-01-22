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
            // NO usar Task.Run aquí directamente si modificamos la UI después
            // Mejor llamar al método async de forma segura
            CargarVehiculosCommand.Execute(null);
        }

        [RelayCommand]
        private async Task CargarVehiculos()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // 1. Llamada a la API (Segundo plano)
                var vehiculos = await _vehiculoService.GetVehiculosAsync();

                // 2. Actualización de la UI (Hilo Principal - OBLIGATORIO EN WINDOWS)
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
                System.Diagnostics.Debug.WriteLine($"Error al cargar: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}