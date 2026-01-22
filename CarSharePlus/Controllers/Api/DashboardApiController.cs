using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlus.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Requiere login (cookie)
    public class DashboardApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalReservas = await _context.Reservas.CountAsync();
            var activas = await _context.Reservas.CountAsync(r => r.Estado == EstadoReserva.Activa);
            var finalizadas = await _context.Reservas.CountAsync(r => r.Estado == EstadoReserva.Finalizada);
            var canceladas = await _context.Reservas.CountAsync(r => r.Estado == EstadoReserva.Cancelada);

            double promedio = 0;
            if (await _context.Reservas.AnyAsync())
            {
                // Cálculo simple de promedio en horas (aproximado para SQL)
                var duraciones = await _context.Reservas
                    .Select(r => EF.Functions.DateDiffMinute(r.FechaInicio, r.FechaFin))
                    .ToListAsync();
                promedio = duraciones.Average() / 60.0;
            }

            return Ok(new
            {
                TotalReservas = totalReservas,
                Activas = activas,
                Finalizadas = finalizadas,
                Canceladas = canceladas,
                PromedioDuracionHoras = promedio
            });
        }
    }
}