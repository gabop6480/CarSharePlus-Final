using CarSharePlusShared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarSharePlusShared.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        public static Usuario? UsuarioActual { get; private set; }

        // El constructor recibe el cliente YA configurado en MauiProgram
        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario?> LoginAsync(string correo, string password)
        {
            try
            {
                var loginData = new { Correo = correo, Password = password };
                // IMPORTANTE: Ruta correcta basada en tu Controller
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginData);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var usuario = await response.Content.ReadFromJsonAsync<Usuario>(options);
                    UsuarioActual = usuario;
                    return usuario;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Login: {ex.Message}");
            }
            return null;
        }

        public Task LogoutAsync()
        {
            UsuarioActual = null;
            return Task.CompletedTask;
        }
    }
}