using CarSharePlusShared.Models;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace CarSharePlusShared.Services
{
    public class EvaluacionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ObservableCollection<Evaluacion> Evaluaciones { get; } = new();

        public EvaluacionService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl + "/api/evaluaciones";
        }

        public async Task CargarEvaluacionesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                if (response.IsSuccessStatusCode)
                {
                    var lista = await response.Content.ReadFromJsonAsync<List<Evaluacion>>();
                    if (lista != null)
                    {
                        Evaluaciones.Clear();
                        foreach (var item in lista) Evaluaciones.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cargando evaluaciones: {ex.Message}");
            }
        }

        public async Task<bool> AgregarEvaluacionAsync(Evaluacion evaluacion)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(_baseUrl, evaluacion);
                if (response.IsSuccessStatusCode)
                {
                    await CargarEvaluacionesAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error agregando evaluación: {ex.Message}");
            }
            return false;
        }
    }
}