using CarSharePlusShared.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace CarSharePlusShared.Services
{
    public class EstadisticasService
    {
        private readonly IReservaService _reservaService;

        public EstadisticasService(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        public int TotalReservas => _reservaService.Reservas.Count;

        public int Activas => _reservaService.Reservas.Count(r => r.Estado == EstadoReserva.Activa);

        public int Finalizadas => _reservaService.Reservas.Count(r => r.Estado == EstadoReserva.Finalizada);

        public int Canceladas => _reservaService.Reservas.Count(r => r.Estado == EstadoReserva.Cancelada);

        public double PromedioDuracionHoras =>
            _reservaService.Reservas.Any()
                ? _reservaService.Reservas.Average(r => (r.FechaFin - r.FechaInicio).TotalHours)
                : 0;
    }
}
