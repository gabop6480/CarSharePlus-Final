using CarSharePlusShared.Models;
using System.Net.Http.Json;

namespace CarSharePlusShared.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public UsuarioService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl + "/api/usuarios";
        }

        public async Task<Usuario?> GetPerfilAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Usuario>($"{_baseUrl}/perfil");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> ActualizarPerfilAsync(Usuario usuario)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/perfil", usuario);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}