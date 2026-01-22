using CarSharePlusShared.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace CarSharePlusShared.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AuthService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl + "/api/auth";
        }

        public async Task<Usuario?> LoginAsync(string correo, string password)
        {
            var loginData = new { Correo = correo, Password = password };
            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var opciones = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                return JsonSerializer.Deserialize<Usuario>(json, opciones);
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            await _httpClient.PostAsync($"{_baseUrl}/logout", null);
        }
    }
}