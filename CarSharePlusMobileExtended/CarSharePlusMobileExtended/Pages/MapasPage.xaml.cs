using CarSharePlusShared.ViewModels;
using CarSharePlusShared.Models;
#if ANDROID || IOS
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
#endif

namespace CarSharePlusMobileExtended.Pages
{
    public partial class MapasPage : ContentPage
    {
        private readonly MapasViewModel _vm;

        public MapasPage(MapasViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await _vm.CargarMapaAsync();

#if ANDROID || IOS
            // Limpiar pines previos
            map.Pins.Clear();

            // Convertir los MapaPin (Shared) en Pin (MAUI)
            foreach (var p in _vm.Pins)
            {
                var pin = new Pin
                {
                    Label = p.Label,
                    Location = new Location(p.Latitud, p.Longitud),
                    Type = p.Tipo == "SavedPin" ? PinType.SavedPin : PinType.Place
                };

                map.Pins.Add(pin);
            }
#else
            // En Windows mostramos un mensaje en lugar de mapa
            await DisplayAlert("Mapa no disponible",
                "La funcionalidad de mapas no está implementada en Windows.",
                "OK");
#endif
        }
    }
}
