using CarSharePlusShared.Models;
using System.Collections.ObjectModel;

namespace CarSharePlusShared.Services
{
    public interface IReservaService
    {
        ObservableCollection<Reserva> Reservas { get; }
        ObservableCollection<Vehiculo> VehiculosDisponibles { get; }

        void AgregarReserva(Reserva reserva);
        void CancelarReserva(Reserva reserva);
        void ActualizarReserva(Reserva reserva);
        void ActualizarEstadoReservas();
        void PrepararEdicion(Reserva reserva); // opcional: almacena estado temporal
    }
}
