using CarSharePlusShared.Models;

namespace CarSharePlusShared.Services
{
    public class PagoService
    {
        public PagoService(HttpClient client) { }

        public async Task<List<Pago>> GetPagosByUsuarioAsync(int usuarioId)
        {
            await Task.Delay(200);

            return new List<Pago>
            {
                new Pago { Id = 101, Monto = 50.00m, FechaPago = DateTime.Now.AddDays(-10), Metodo = "Tarjeta Visa", Confirmado = true },
                new Pago { Id = 102, Monto = 120.50m, FechaPago = DateTime.Now.AddDays(-5), Metodo = "PayPal", Confirmado = true },
                new Pago { Id = 103, Monto = 15.00m, FechaPago = DateTime.Now.AddDays(-1), Metodo = "Efectivo", Confirmado = false }
            };
        }
    }
}