using CarSharePlusShared.Models;
using CarSharePlusShared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CarSharePlusShared.ViewModels
{
    public partial class MapasViewModel : ObservableObject
    {
        private readonly OverpassService _overpassService;

        [ObservableProperty]
        private ObservableCollection<MapaPin> pins = new();

        public MapasViewModel(OverpassService overpassService)
        {
            _overpassService = overpassService;
        }

        [RelayCommand]
        public async Task CargarMapaAsync()
        {
            try
            {
                // Vehículos de prueba
                var vehiculos = new List<Vehiculo>
                {
                    new Vehiculo { Placa = "ABC123", Latitud = -0.1807, Longitud = -78.4678 },
                    new Vehiculo { Placa = "XYZ789", Latitud = -0.1850, Longitud = -78.4800 }
                };

                foreach (var v in vehiculos)
                {
                    Pins.Add(new MapaPin
                    {
                        Label = $"🚗 Vehículo {v.Placa}",
                        Latitud = v.Latitud,
                        Longitud = v.Longitud,
                        Tipo = "Place"
                    });
                }

                // Aquí ya no usamos Geolocation de MAUI en Shared.
                // Podrías inyectar la ubicación desde Mobile y pasarla como parámetro.
                var ubicacionLat = -0.1807;
                var ubicacionLon = -78.4678;

                Pins.Add(new MapaPin
                {
                    Label = "📍 Tu ubicación",
                    Latitud = ubicacionLat,
                    Longitud = ubicacionLon,
                    Tipo = "SavedPin"
                });

                // Gasolineras
                var gasolineras = await _overpassService.BuscarLugaresAsync(ubicacionLat, ubicacionLon, "fuel");
                foreach (var lugar in gasolineras)
                {
                    Pins.Add(new MapaPin
                    {
                        Label = "⛽ Gasolinera",
                        Latitud = lugar.Latitud,
                        Longitud = lugar.Longitud,
                        Tipo = "Place"
                    });
                }

                // Electrolineras
                var electrolineras = await _overpassService.BuscarLugaresAsync(ubicacionLat, ubicacionLon, "charging_station");
                foreach (var lugar in electrolineras)
                {
                    Pins.Add(new MapaPin
                    {
                        Label = "🔌 Electrolinera",
                        Latitud = lugar.Latitud,
                        Longitud = lugar.Longitud,
                        Tipo = "Place"
                    });
                }
            }
            catch (Exception ex)
            {
                // 🚫 No usar DisplayAlert aquí
                throw new InvalidOperationException($"No se pudo cargar el mapa: {ex.Message}");
            }
        }
    }
}
