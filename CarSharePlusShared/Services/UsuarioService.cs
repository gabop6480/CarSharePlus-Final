using System.Net.Http.Json;
using CarSharePlusShared.Models;

namespace CarSharePlusShared.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Usuario>($"api/usuarios/{id}");
            }
            catch
            {
                return null;
            }
        }
    }
}