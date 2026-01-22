using System.Net.Http.Json;
using CarSharePlusShared.Models;
using Microsoft.Maui.Devices;

namespace CarSharePlusShared.Services
{
    public class UsuarioService
    {
        private readonly HttpClient _httpClient;

        public UsuarioService()
        {
            string urlBase = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5136" : "http://localhost:5136";
            _httpClient = new HttpClient { BaseAddress = new Uri(urlBase) };
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            try { return await _httpClient.GetFromJsonAsync<Usuario>($"api/usuarios/{id}"); }
            catch { return null; }
        }
    }
}