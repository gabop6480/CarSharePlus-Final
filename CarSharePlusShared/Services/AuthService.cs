using CarSharePlusShared.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Maui.Devices; // Necesario para detectar plataforma

namespace CarSharePlusShared.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService()
        {
            // 1. Configurar URL según el dispositivo
            // Android Emulator usa 10.0.2.2 para ver el localhost de tu PC
            string urlBase = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5136/api/auth"
                : "http://localhost:5136/api/auth";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(urlBase),
                Timeout = TimeSpan.FromSeconds(30) // Dar tiempo si la red es lenta
            };
        }

        public async Task<Usuario?> LoginAsync(string correo, string password)
        {
            try
            {
                var loginData = new { Correo = correo, Password = password };

                // 2. CONFIGURACIÓN CRÍTICA:
                // PropertyNamingPolicy = null evita que cambie a minúsculas (camelCase).
                // Envía "Correo" en lugar de "correo".
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null
                };

                var response = await _httpClient.PostAsJsonAsync("auth/login", loginData, jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    // Al recibir, sí permitimos mayúsculas o minúsculas por flexibilidad
                    var readOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    return await response.Content.ReadFromJsonAsync<Usuario>(readOptions);
                }
                else
                {
                    // (Opcional) Leer error para depurar si falla
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Login: {error}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción Login: {ex.Message}");
                return null;
            }
        }

        public Task LogoutAsync()
        {
            // Limpieza local si fuera necesaria
            return Task.CompletedTask;
        }
    }
}