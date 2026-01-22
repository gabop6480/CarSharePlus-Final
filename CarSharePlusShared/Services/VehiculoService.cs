using System.Net.Http.Json;
using CarSharePlusShared.Models;
using System.Text.Json;

namespace CarSharePlusShared.Services
{
    public class VehiculoService
    {
        private readonly HttpClient _httpClient;

        public VehiculoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                // Esta llamada ahora incluye automáticamente la cookie de sesión
                return await _httpClient.GetFromJsonAsync<List<Vehiculo>>("api/vehiculos", options) ?? new List<Vehiculo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Vehiculos: {ex.Message}");
                return new List<Vehiculo>();
            }
        }
    }
}