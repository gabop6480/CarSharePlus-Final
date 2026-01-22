using System.Net.Http.Json;

namespace CarSharePlusShared.Services
{
    public class DashboardStats
    {
        public int TotalReservas { get; set; }
        public int Activas { get; set; }
        public int Finalizadas { get; set; }
        public int Canceladas { get; set; }
        public double PromedioDuracionHoras { get; set; }
    }

    public class DashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public DashboardService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl + "/api/dashboard";
        }

        public async Task<DashboardStats?> GetStatsAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DashboardStats>($"{_baseUrl}/stats");
            }
            catch
            {
                return new DashboardStats(); // Retorna vacío si hay error
            }
        }
    }
}