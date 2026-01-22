using System.Net.Http.Json;
using CarSharePlusShared.Models;

namespace CarSharePlusShared.Services
{
    public class VehiculoService
    {
        private readonly HttpClient _httpClient;

        // Constructor recibe el cliente compartido
        public VehiculoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            try
            {
                // Como usamos el cliente compartido, la cookie de sesión va incluida aquí
                return await _httpClient.GetFromJsonAsync<List<Vehiculo>>("api/vehiculos") ?? new List<Vehiculo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Vehiculos: {ex.Message}");
                return new List<Vehiculo>();
            }
        }
    }
}