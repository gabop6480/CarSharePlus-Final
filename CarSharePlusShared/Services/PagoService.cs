using CarSharePlusShared.Models;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;

namespace CarSharePlusShared.Services
{
    public class PagoService : IPagoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public ObservableCollection<Pago> Pagos { get; } = new();

        private readonly string _baseUrl;

        public PagoService(HttpClient httpClient, string baseUrl = "https://tuservidor/api/pagos")
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = baseUrl;
        }

        public async Task<bool> RegistrarPago(Pago pago)
        {
            try
            {
                if (pago.Id == 0) pago.Id = Pagos.Count + 1;
                pago.FechaPago = DateTime.Now;

                var json = JsonSerializer.Serialize(pago, _jsonOptions);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_baseUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var pagoApi = JsonSerializer.Deserialize<Pago>(responseJson, _jsonOptions);
                    if (pagoApi != null) pago.Id = pagoApi.Id;
                    pago.Confirmado = true;
                }
                else
                {
                    pago.Confirmado = false;
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Error API al registrar pago: {errorMsg}");
                }

                Pagos.Add(pago);
                return pago.Confirmado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción al registrar pago: {ex.Message}");
                pago.Confirmado = false;
                Pagos.Add(pago);
                return false;
            }
        }

        public async Task<bool> CargarPagosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var pagosDb = JsonSerializer.Deserialize<List<Pago>>(json, _jsonOptions);

                if (pagosDb != null)
                {
                    Pagos.Clear();
                    foreach (var pago in pagosDb)
                        Pagos.Add(pago);
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar pagos: {ex.Message}");
                return false;
            }
        }

        public async Task<decimal> CalcularMontoPorReserva(int reservaId)
        {
            await Task.Delay(10); // simulación async
            decimal tarifaPorHora = 10;
            int horas = 3;
            return tarifaPorHora * horas;
        }

        public async Task<bool> ExistePagoParaReserva(int reservaId)
        {
            await Task.Delay(1);
            return Pagos.Any(p => p.ReservaId == reservaId && p.Confirmado);
        }

        public async Task<bool> ConfirmarPago(int pagoId)
        {
            var pago = Pagos.FirstOrDefault(p => p.Id == pagoId);
            if (pago == null) return false;

            pago.Confirmado = true;
            await Task.Delay(1);
            return true;
        }
    }
}
