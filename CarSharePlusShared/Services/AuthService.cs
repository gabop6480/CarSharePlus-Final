using CarSharePlusShared.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Maui.Devices;

namespace CarSharePlusShared.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            // CORRECCIÓN: La URL base NO debe incluir "/api/auth" todavía.
            // Solo la dirección del servidor y el puerto.
            string urlBase = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5136"  // Android Emulator
                : "http://localhost:5136"; // Windows

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(urlBase),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<Usuario?> LoginAsync(string correo, string password)
        {
            try
            {
                var loginData = new { Correo = correo, Password = password };

                // Opciones para enviar mayúsculas exactas
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                // Enviamos la petición a la ruta COMPLETA aquí
                // Resultado final: http://localhost:5136/api/auth/login
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData, jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    var readOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return await response.Content.ReadFromJsonAsync<Usuario>(readOptions);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[ERROR LOGIN] Status: {response.StatusCode}, Detalles: {error}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPCIÓN LOGIN] {ex.Message}");
                return null;
            }
        }

        public Task LogoutAsync() => Task.CompletedTask;
    }
}