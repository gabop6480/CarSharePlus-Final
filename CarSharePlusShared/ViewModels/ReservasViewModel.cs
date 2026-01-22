using CarSharePlusShared.Models;
using CarSharePlusShared.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CarSharePlusShared.ViewModels
{
    public partial class ReservasViewModel : ObservableObject
    {
        private readonly IReservaService _reservaService;

        [ObservableProperty]
        private ObservableCollection<Reserva> reservas = new();

        public ReservasViewModel(IReservaService reservaService)
        {
            _reservaService = reservaService;
            _reservaService.ActualizarEstadoReservas();
            Reservas = _reservaService.Reservas;
        }

        public event EventHandler<string>? OperacionCompletada;
        public event EventHandler<Reserva>? SolicitarEdicion; // Mobile navega cuando recibe esto

        [RelayCommand]
        private Task CancelarReservaAsync(Reserva reserva)
        {
            if (reserva == null) return Task.CompletedTask;

            try
            {
                _reservaService.CancelarReserva(reserva);
                OperacionCompletada?.Invoke(this, "Reserva cancelada correctamente.");
            }
            catch (Exception ex)
            {
                OperacionCompletada?.Invoke(this, $"No se pudo cancelar la reserva: {ex.Message}");
            }

            return Task.CompletedTask;
        }

        [RelayCommand]
        private Task EditarReservaAsync(Reserva reserva)
        {
            if (reserva == null) return Task.CompletedTask;

            _reservaService.PrepararEdicion(reserva);
            SolicitarEdicion?.Invoke(this, reserva);
            return Task.CompletedTask;
        }
    }
}
