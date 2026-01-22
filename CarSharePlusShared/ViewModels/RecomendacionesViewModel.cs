using CarSharePlusShared.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace CarSharePlusShared.ViewModels
{
    public partial class RecomendacionesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<VehiculoRecomendado> vehiculosRecomendados = new();

        [RelayCommand]
        private async Task CargarRecomendaciones()
        {
            try
            {
                using var client = new HttpClient();
                var json = await client.GetStringAsync("https://tuservidor/api/dashboard/rankingvehiculos");
                var lista = JsonSerializer.Deserialize<List<VehiculoRecomendado>>(json) ?? new List<VehiculoRecomendado>();

                VehiculosRecomendados.Clear();
                foreach (var v in lista) VehiculosRecomendados.Add(v);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"No se pudieron cargar las recomendaciones: {ex.Message}");
            }
        }
    }
}
