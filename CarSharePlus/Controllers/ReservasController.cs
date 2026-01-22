using CarSharePlus.Data;
using CarSharePlusShared.Models;
using CarSharePlusShared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlusWeb.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===================== INDEX =====================
        public IActionResult Index()
        {
            return RedirectToAction(nameof(MisReservas));
        }

        // ===================== MIS RESERVAS =====================
        public async Task<IActionResult> MisReservas()
        {
            var correo = User.Identity?.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null) return Unauthorized();

            var reservas = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .Where(r => r.UsuarioId == usuario.Id)
                .Select(r => new ReservaViewModel
                {
                    Id = r.Id,
                    UsuarioId = r.UsuarioId,
                    UsuarioNombre = r.Usuario.Nombre,
                    VehiculoId = r.VehiculoId,
                    VehiculoPlaca = r.Vehiculo.Placa,
                    FechaInicio = r.FechaInicio,
                    FechaFin = r.FechaFin,
                    UbicacionInicio = r.UbicacionInicio ?? "",
                    UbicacionFin = r.UbicacionFin ?? "",
                    Estado = r.Estado,
                    TarifaPorHora = r.Vehiculo.TarifaPorHora,
                    MontoPago = r.MontoPago,
                    Calificacion = r.Calificacion ?? 0,
                    Comentario = r.Comentario ?? ""
                })
                .ToListAsync();

            return View("Index", reservas);
        }

        // ===================== CREATE GET =====================
        // ===================== CREATE GET =====================
        public IActionResult Create()
        {
            // Traemos solo los vehículos disponibles
            var vehiculosDisponibles = _context.Vehiculos
                .Where(v => v.Disponible)
                .Select(v => new { v.Id, v.Placa, v.Marca, v.Modelo, v.TarifaPorHora, v.Transmision, v.Energia, v.Longitud, v.Latitud })
                .ToList();

            if (!vehiculosDisponibles.Any())
            {
                TempData["ErrorMessage"] = "No hay vehículos disponibles en este momento.";
                return RedirectToAction("Index", "Home");
            }

            // SelectList para el dropdown en la vista
            var listaVehiculos = 
                vehiculosDisponibles.Select(v => new SelectListItem 
                { 
                    Value = v.Id.ToString(), Text = $"{v.Placa} - {v.Marca} {v.Modelo} | {v.Transmision}, {v.Energia} | ${v.TarifaPorHora}/h" 
                }).ToList();

            ViewBag.Vehiculos = listaVehiculos;
            // Creamos un viewmodel inicial con fechas por defecto
            var vm = new ReservaViewModel
            {
                FechaInicio = DateTime.Now,
                FechaFin = DateTime.Now.AddHours(2)
            };

            return View(vm);
        }


        // ===================== CREATE POST =====================
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservaViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // 👤 Usuario que está reservando
            var correo = User.Identity?.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
                return Unauthorized();

            // 🚗 Buscar vehículo SIN filtrar por propietario
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Id == vm.VehiculoId);

            if (vehiculo == null)
            {
                ModelState.AddModelError("", "Vehículo no encontrado");
                return View(vm);
            }

            if (!vehiculo.Disponible)
            {
                ModelState.AddModelError("", "El vehículo ya no está disponible");
                return View(vm);
            }

            // ⏱ Validar fechas
            var horas = (vm.FechaFin - vm.FechaInicio).TotalHours;

            if (horas <= 0)
            {
                ModelState.AddModelError("", "Fechas inválidas");
                return View(vm);
            }

            // 🧠 Crear reserva
            var reserva = vm.ToModel();

            reserva.UsuarioId = usuario.Id;   // 👉 EL QUE RESERVA
            reserva.Estado = EstadoReserva.Pendiente;

            reserva.MontoPago = Math.Round(
                (decimal)horas * vehiculo.TarifaPorHora,
                2
            );

            reserva.Comentario ??= "";

            // 🔐 Bloquear vehículo
            vehiculo.Disponible = false;

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // 💳 Crear pago automático
            var pago = new Pago
            {
                ReservaId = reserva.Id,
                Monto = reserva.MontoPago,
                Metodo = "Pendiente",
                Confirmado = false
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Reserva creada correctamente. Total a pagar: ${reserva.MontoPago}";

            return RedirectToAction(nameof(MisReservas));
        }


        // ===================== EDIT GET =====================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UsuarioId == usuario.Id);   // 🔐 SOLO SU RESERVA

            if (reserva == null) return NotFound();

            var vm = ReservaViewModel.FromModel(reserva);

            ViewData["Vehiculos"] = new SelectList(
                _context.Vehiculos.Where(v => v.Disponible || v.Id == vm.VehiculoId),
                "Id",
                "Placa",
                vm.VehiculoId
            );

            return View(vm);
        }

        // ===================== EDIT POST =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReservaViewModel vm)
        {
            if (id != vm.Id) return NotFound();

            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null) return Unauthorized();

            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UsuarioId == usuario.Id);

            if (reserva == null) return NotFound();

            // Buscar vehículo seleccionado
            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Id == vm.VehiculoId);

            if (vehiculo == null)
            {
                ModelState.AddModelError("", "Vehículo no válido");
                ViewData["Vehiculos"] = new SelectList(
                    _context.Vehiculos.Where(v => v.Disponible || v.Id == vm.VehiculoId),
                    "Id", "Placa", vm.VehiculoId
                );
                return View(vm);
            }

            // Calcular horas
            var horas = (vm.FechaFin - vm.FechaInicio).TotalHours;
            if (horas <= 0)
            {
                ModelState.AddModelError("", "Fechas inválidas");
                ViewData["Vehiculos"] = new SelectList(
                    _context.Vehiculos.Where(v => v.Disponible || v.Id == vm.VehiculoId),
                    "Id", "Placa", vm.VehiculoId
                );
                return View(vm);
            }

            // 🔹 Detectar cambios importantes
            bool cambioImportante =
                reserva.VehiculoId != vm.VehiculoId ||
                reserva.FechaInicio != vm.FechaInicio ||
                reserva.FechaFin != vm.FechaFin;

            const decimal MULTA_CAMBIO = 5.00m;
            decimal nuevoMonto = Math.Round((decimal)horas * vehiculo.TarifaPorHora, 2);

            if (cambioImportante)
            {
                nuevoMonto += MULTA_CAMBIO;

                // Liberar vehículo anterior
                if (reserva.Vehiculo != null)
                    reserva.Vehiculo.Disponible = true;

                // Bloquear nuevo vehículo
                vehiculo.Disponible = false;
                reserva.VehiculoId = vm.VehiculoId;
            }

            // Actualizar reserva
            reserva.FechaInicio = vm.FechaInicio;
            reserva.FechaFin = vm.FechaFin;
            reserva.UbicacionInicio = vm.UbicacionInicio;
            reserva.UbicacionFin = vm.UbicacionFin;
            reserva.Estado = vm.Estado;
            reserva.Calificacion = vm.Calificacion;
            reserva.Comentario = vm.Comentario ?? "";

            // ⚡ Ignorar monto enviado por el formulario y recalcularlo
            reserva.MontoPago = nuevoMonto;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Reserva actualizada correctamente. Total a pagar: ${reserva.MontoPago}";

            return RedirectToAction(nameof(MisReservas));
        }


        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.Id == id && r.UsuarioId == usuario.Id);

            if (reserva == null) return NotFound();

            // Usamos el ViewModel
            var vm = ReservaViewModel.FromModel(reserva);
            return View(vm); // Esto abre la vista Delete.cshtml
        }

        // ===================== DELETE =====================
        // ================= DELETE POST REAL =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var correo = User.Identity?.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UsuarioId == usuario.Id);

            if (reserva == null)
                return NotFound();

            // 🔁 Liberar vehículo
            if (reserva.Vehiculo != null)
                reserva.Vehiculo.Disponible = true;

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva eliminada correctamente";

            return RedirectToAction(nameof(MisReservas));
        }


        // ================= CANCELAR =================
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            var reserva = await _context.Reservas
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UsuarioId == usuario.Id);

            if (reserva == null)
                return NotFound();

            reserva.Vehiculo.Disponible = true;
            reserva.Estado = EstadoReserva.Cancelada;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Reserva cancelada y vehículo liberado.";

            return RedirectToAction(nameof(MisReservas));
        }

        // ===================== DETAILS =====================
        // ===================== DETAILS =====================
        
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var correo = User.Identity?.Name;

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
                return Unauthorized();

            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .Include(r => r.Pagos)
                .FirstOrDefaultAsync(r =>
                    r.Id == id &&
                    r.UsuarioId == usuario.Id);

            if (reserva == null)
                return NotFound();

            // 🔥 AQUÍ ESTÁ LA CLAVE
            var vm = ReservaViewModel.FromModel(reserva);

            return View(vm);
        }

        // GET: Reservas/GetVehiculoDetalles/5
        public async Task<IActionResult> GetVehiculoDetalles(int id)
        {
            var v = await _context.Vehiculos
                .Include(v => v.Usuario)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (v == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                marca = v.Marca,
                modelo = v.Modelo,
                placa = v.Placa,
                tarifaPorHora = v.TarifaPorHora,
                dueno = v.Usuario.Nombre,
                disponible = v.Disponible
            });
        }


    }
}
