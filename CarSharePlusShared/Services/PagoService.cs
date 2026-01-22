using System.Net.Http.Json;
using CarSharePlusShared.Models;

namespace CarSharePlusShared.Services
{
    public class PagoService
    {
        private readonly HttpClient _httpClient;

        public PagoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Pago>> GetPagosByUsuarioAsync(int usuarioId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Pago>>($"api/pagos/mis-pagos") ?? new List<Pago>();
            }
            catch
            {
                return new List<Pago>();
            }
        }
    }
}