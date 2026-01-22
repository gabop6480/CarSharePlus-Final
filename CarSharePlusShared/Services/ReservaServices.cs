using CarSharePlusShared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarSharePlusShared.Services
{
    public class ReservaService : IReservaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public ObservableCollection<Reserva> Reservas { get; } = new();
        public ObservableCollection<Vehiculo> VehiculosDisponibles { get; } = new();

        public ReservaService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
            _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        // Cargar Reservas desde la API
        public async Task CargarReservasAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/reservas");
                if (response.IsSuccessStatusCode)
                {
                    var lista = await response.Content.ReadFromJsonAsync<List<Reserva>>(_jsonOptions);
                    if (lista != null)
                    {
                        Reservas.Clear();
                        foreach (var item in lista) Reservas.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando reservas: {ex.Message}");
            }
        }

        public async void AgregarReserva(Reserva reserva)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/reservas", reserva);
            if (response.IsSuccessStatusCode)
            {
                await CargarReservasAsync();
            }
        }

        public async void CancelarReserva(Reserva reserva)
        {
            // Implementación simplificada: eliminar o cambiar estado
            await _httpClient.DeleteAsync($"{_baseUrl}/api/reservas/{reserva.Id}");
            await CargarReservasAsync();
        }

        public async void ActualizarReserva(Reserva reserva)
        {
            await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/reservas/{reserva.Id}", reserva);
            await CargarReservasAsync();
        }

        // Métodos auxiliares de interfaz
        public void ActualizarEstadoReservas() { /* Lógica manejada por backend */ }
        public void PrepararEdicion(Reserva reserva) { /* Implementar según necesidad de UI */ }
    }
}