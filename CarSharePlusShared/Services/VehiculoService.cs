using CarSharePlusShared.Models;

namespace CarSharePlusShared.Services
{
    public class VehiculoService
    {
        // No necesitamos HttpClient para datos falsos
        public VehiculoService(HttpClient httpClient) { }

        public async Task<List<Vehiculo>> GetVehiculosAsync()
        {
            // Simulamos una pequeña espera para que parezca real
            await Task.Delay(500);

            return new List<Vehiculo>
            {
                new Vehiculo
                {
                    Id = 1,
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Anio = 2024,
                    Placa = "ABC-123",
                    TarifaPorHora = 15.50m,
                    Disponible = true
                },
                new Vehiculo
                {
                    Id = 2,
                    Marca = "Tesla",
                    Modelo = "Model 3",
                    Anio = 2023,
                    Placa = "XYZ-987",
                    TarifaPorHora = 25.00m,
                    Disponible = true
                },
                new Vehiculo
                {
                    Id = 3,
                    Marca = "Ford",
                    Modelo = "Explorer",
                    Anio = 2022,
                    Placa = "FGH-456",
                    TarifaPorHora = 20.00m,
                    Disponible = false
                },
                new Vehiculo
                {
                    Id = 4,
                    Marca = "Chevrolet",
                    Modelo = "Camaro",
                    Anio = 2021,
                    Placa = "CHE-999",
                    TarifaPorHora = 35.00m,
                    Disponible = true
                }
            };
        }
    }
}