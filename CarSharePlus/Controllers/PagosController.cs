using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlus.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // INDEX → ADMIN
        // =====================================================
        public async Task<IActionResult> Index()
        {
            var pagos = await _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Usuario)
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                .Take(100)
                .ToListAsync();

            return View(pagos);
        }

        // =====================================================
        // CREATE GET → Preparar formulario con reserva
        // =====================================================
        public async Task<IActionResult> Create(int reservaId)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                    .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null)
                return NotFound();

            var pago = new Pago
            {
                ReservaId = reserva.Id,
                FechaPago = DateTime.Now,
                Confirmado = false,
                Reserva = reserva,
                Monto = Math.Round((decimal)(reserva.FechaFin - reserva.FechaInicio).TotalHours * reserva.Vehiculo.TarifaPorHora, 2)
            };

            return View(pago);
        }

        // =====================================================
        // CREATE POST → Procesar pago
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(int reservaId, string metodo)
        {
            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null) return NotFound();
            if (reserva.UsuarioId != usuario.Id) return Forbid();

            var existePagoConfirmado = await _context.Pagos
                .AnyAsync(p => p.ReservaId == reservaId && p.Confirmado);

            if (existePagoConfirmado)
            {
                TempData["ErrorMessage"] = "Ya existe un pago confirmado para esta reserva.";
                return RedirectToAction("MisReservas", "Reservas");
            }

            // Calcular monto automáticamente
            var horas = (reserva.FechaFin - reserva.FechaInicio).TotalHours;
            if (horas <= 0)
            {
                TempData["ErrorMessage"] = "Fechas de reserva inválidas.";
                return RedirectToAction("MisReservas", "Reservas");
            }

            var pago = new Pago
            {
                ReservaId = reserva.Id,
                Metodo = metodo,
                Monto = Math.Round((decimal)horas * reserva.Vehiculo.TarifaPorHora, 2),
                FechaPago = DateTime.Now,
                Confirmado = true
            };

            _context.Pagos.Add(pago);

            reserva.MontoPago = pago.Monto;
            reserva.Estado = EstadoReserva.Activa;
            reserva.Vehiculo.Disponible = false;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Pago realizado. Reserva activada por ${pago.Monto}";
            return RedirectToAction("MisReservas", "Reservas");
        }

        // =====================================================
        // MIS PAGOS → Solo pagos propios o donde soy dueño
        // =====================================================
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MisPagos()
        {
            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
                return RedirectToAction("Index", "Home");

            // Traer solo pagos del usuario o donde es dueño del vehículo
            var pagos = await _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                        .ThenInclude(v => v.Usuario)
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Usuario)
                .Where(p => p.Reserva.UsuarioId == usuario.Id
                         || p.Reserva.Vehiculo.UsuarioId == usuario.Id)
                .AsNoTracking()
                .ToListAsync();

            // ViewBag para saber quién es el usuario actual
            ViewBag.UsuarioActual = usuario.Id;

            return View(pagos);
        }


        // =====================================================
        // DELETE → No si está confirmado
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null) return RedirectToAction(nameof(Index));
            if (pago.Confirmado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar un pago confirmado.";
                return RedirectToAction(nameof(Index));
            }

            _context.Pagos.Remove(pago);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Pago eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DETAILS → Solo propietario o arrendatario
        // =====================================================
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int id)
        {
            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Usuario)
                .Include(p => p.Reserva)
                    .ThenInclude(r => r.Vehiculo)
                        .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pago == null) return NotFound();

            var correo = User.Identity?.Name;
            if (pago.Reserva.Usuario.Correo != correo && pago.Reserva.Vehiculo.Usuario.Correo != correo)
                return Forbid();

            return View(pago);
        }
    }
}
