using CarSharePlus.Data;
using Microsoft.AspNetCore.Mvc;
using CarSharePlusShared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace CarSharePlus.Controllers
{
    [Authorize(Roles = "Admin")] // 🔒 Solo Admin puede acceder
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 📊 Promedio de calificaciones por vehículo (Marca + Modelo)
            var promedioPorVehiculo = await _context.Evaluaciones
                .Include(e => e.Vehiculo)
                .GroupBy(e => e.Vehiculo.Id)
                .Select(g => new
                {
                    Vehiculo = g.First().Vehiculo.Marca + " " + g.First().Vehiculo.Modelo,
                    Promedio = g.Average(e => e.Calificacion)
                })
                .OrderByDescending(g => g.Promedio)
                .ToListAsync();

            // 🔹 Promedio global de todas las evaluaciones
            double promedioGlobal = await _context.Evaluaciones.AnyAsync()
                ? await _context.Evaluaciones.AverageAsync(e => e.Calificacion)
                : 0;

            ViewBag.PromedioGlobal = promedioGlobal;

            // 👥 Top 3 usuarios más activos (por cantidad de evaluaciones)
            var topUsuarios = await _context.Evaluaciones
                .GroupBy(e => e.UsuarioId)
                .Select(g => new
                {
                    Usuario = g.First().Usuario.Nombre,
                    Total = g.Count()
                })
                .OrderByDescending(g => g.Total)
                .Take(3)
                .ToListAsync();

            // ⭐ Distribución de calificaciones (1-5)
            var distribucion = await _context.Evaluaciones
                .GroupBy(e => e.Calificacion)
                .Select(g => new
                {
                    Calificacion = g.Key,
                    Total = g.Count()
                })
                .OrderBy(g => g.Calificacion)
                .ToListAsync();

            // 🚗 Ranking de vehículos recomendados (Top 5 por promedio)
            var rankingVehiculos = promedioPorVehiculo.Take(5).ToList();

            // 📌 Reservas con calificación (para gráfico)
            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .AsNoTracking()
                .ToListAsync();

            var distribucionReservas = reservas
                .Where(r => r.Calificacion.HasValue)
                .GroupBy(r => r.Calificacion.Value)
                .Select(g => new
                {
                    Calificacion = g.Key,
                    Total = g.Count()
                })
                .OrderBy(g => g.Calificacion)
                .ToList();

            // ✅ Preparar listas para Chart.js
            ViewBag.ReservaLabels = distribucionReservas.Select(d => d.Calificacion).ToList();
            ViewBag.ReservaValues = distribucionReservas.Select(d => d.Total).ToList();

            // Pasamos todo a la vista
            ViewBag.PromedioPorVehiculo = promedioPorVehiculo;
            ViewBag.TopUsuarios = topUsuarios;
            ViewBag.Distribucion = distribucion;
            ViewBag.RankingVehiculos = rankingVehiculos;

            return View();
        }
    }
}
