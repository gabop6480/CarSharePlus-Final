using CarSharePlusShared.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CarSharePlusShared.Services
{
    public interface IPagoService
    {
        ObservableCollection<Pago> Pagos { get; }

        Task<bool> RegistrarPago(Pago pago);
        Task<bool> CargarPagosAsync();
        Task<decimal> CalcularMontoPorReserva(int reservaId);
        Task<bool> ExistePagoParaReserva(int reservaId);
        Task<bool> ConfirmarPago(int pagoId);
    }
}
