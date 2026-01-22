using CarSharePlus.Data;
using CarSharePlusShared.Models;
using CarSharePlusShared.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class VehiculosController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiculosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Vehiculos
    public async Task<IActionResult> Index(string searchBrand, TipoTransmision? transmision, TipoEnergia? energia, int? anioDesde, int? anioHasta, bool? disponible)
    {
        var correo = User.Identity?.Name;
        var usuarioActual = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuarioActual == null)
            return Unauthorized();

        var vehiculos = _context.Vehiculos.Include(v => v.Usuario).AsQueryable();

        if (!string.IsNullOrEmpty(searchBrand))
            vehiculos = vehiculos.Where(v => v.Marca.ToLower().Contains(searchBrand.ToLower()));

        if (transmision.HasValue)
            vehiculos = vehiculos.Where(v => v.Transmision == transmision.Value);

        if (energia.HasValue)
            vehiculos = vehiculos.Where(v => v.Energia == energia.Value);

        if (anioDesde.HasValue)
            vehiculos = vehiculos.Where(v => v.Anio >= anioDesde.Value);

        if (anioHasta.HasValue)
            vehiculos = vehiculos.Where(v => v.Anio <= anioHasta.Value);

        if (disponible.HasValue)
            vehiculos = vehiculos.Where(v => v.Disponible == disponible.Value);

        var lista = await vehiculos.AsNoTracking().ToListAsync();
        var vmLista = lista.Select(VehiculoViewModel.FromModel).ToList();

        if (vmLista.Count == 0)
            TempData["InfoMessage"] = "🚗 No hay vehículos registrados.";

        return View(vmLista);
    }

    // GET: Vehiculos/Create
    [Authorize]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Vehiculos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(VehiculoViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        if (await _context.Vehiculos.AnyAsync(v => v.Placa == model.Placa))
        {
            ModelState.AddModelError("Placa", "La placa ya está registrada.");
            return View(model);
        }

        var correo = User.Identity?.Name;
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuario == null)
        {
            TempData["ErrorMessage"] = "Usuario no encontrado.";
            return RedirectToAction("Index", "Home");
        }

        var vehiculo = model.ToModel(usuario.Id);
        // 🔥 asignación automática

        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Vehículo creado correctamente ✅";
        return RedirectToAction(nameof(MisVehiculos));
    }

    // GET: Vehiculos/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        // ✅ Obtener usuario logueado REAL
        var correo = User.Identity?.Name;
        var usuarioActual = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuarioActual == null)
            return Unauthorized();

        var vehiculo = await _context.Vehiculos
            .Include(v => v.Usuario)
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                v.UsuarioId == usuarioActual.Id);   // 🔐 SEGURIDAD REAL

        if (vehiculo == null)
            return NotFound();

        var vm = new VehiculoViewModel
        {
            Id = vehiculo.Id,
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Anio = vehiculo.Anio,
            Placa = vehiculo.Placa,
            Transmision = vehiculo.Transmision,
            Energia = vehiculo.Energia,
            AutonomiaKm = vehiculo.AutonomiaKm,
            ConsumoPorKm = vehiculo.ConsumoPorKm,
            Disponible = vehiculo.Disponible,
            UsuarioId = vehiculo.UsuarioId,
            TarifaPorHora = vehiculo.TarifaPorHora,

            UsuarioNombre = vehiculo.Usuario?.Nombre,
            UsuarioCorreo = vehiculo.Usuario?.Correo
        };

        // 👇 Combo SOLO con el usuario actual
        ViewBag.UsuarioId = new SelectList(
            _context.Usuarios.Where(u => u.Id == usuarioActual.Id),
            "Id",
            "Correo",
            vehiculo.UsuarioId
        );

        return View(vm);
    }

    // POST: Vehiculos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int id, VehiculoViewModel model)
    {
        if (id != model.Id)
            return BadRequest("El ID no coincide con el vehículo a editar");

        // 🔐 1. Obtener usuario logueado
        var correo = User.Identity?.Name;
        var usuarioActual = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuarioActual == null)
            return Unauthorized();

        // 🔐 2. SOLO buscar vehículo DEL USUARIO
        var vehiculo = await _context.Vehiculos
            .FirstOrDefaultAsync(v =>
                v.Id == id &&
                v.UsuarioId == usuarioActual.Id);

        if (vehiculo == null)
            return NotFound();

        // 🔁 Si hay error → recargar combo
        if (!ModelState.IsValid)
        {
            ViewBag.UsuarioId = new SelectList(
                _context.Usuarios.Where(u => u.Id == usuarioActual.Id),
                "Id",
                "Correo",
                model.UsuarioId
            );

            return View(model);
        }

        try
        {
            vehiculo.Marca = model.Marca;
            vehiculo.Modelo = model.Modelo;
            vehiculo.Placa = model.Placa;
            vehiculo.Anio = model.Anio;
            vehiculo.Transmision = model.Transmision;
            vehiculo.Energia = model.Energia;
            vehiculo.Disponible = model.Disponible;
            vehiculo.AutonomiaKm = model.AutonomiaKm;
            vehiculo.ConsumoPorKm = model.ConsumoPorKm;
            vehiculo.TarifaPorHora = model.TarifaPorHora;

            // 🚫 NO permitir cambiar dueño
            // vehiculo.UsuarioId = model.UsuarioId;  ← PROHIBIDO

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Vehículo actualizado correctamente ✏️";

            return RedirectToAction(nameof(MisVehiculos));
        }
        catch (DbUpdateConcurrencyException)
        {
            TempData["ErrorMessage"] =
                "Conflicto de concurrencia al actualizar el vehículo.";

            ViewBag.UsuarioId = new SelectList(
                _context.Usuarios.Where(u => u.Id == usuarioActual.Id),
                "Id",
                "Correo",
                model.UsuarioId
            );

            return View(model);
        }
    }

    [Authorize]
    public async Task<IActionResult> MisVehiculos()
    {
        var correo = User.Identity?.Name;

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuario == null)
            return Unauthorized();

        var vehiculos = await _context.Vehiculos
            .Include(v => v.Usuario)
            .Where(v => v.UsuarioId == usuario.Id)
            .ToListAsync();

        // 🔥 AQUÍ ESTÁ LA CLAVE
        var viewModels = vehiculos
            .Select(VehiculoViewModel.FromModel)
            .ToList();

        return View(viewModels);
    }

    // GET: Vehiculos/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var vehiculo = await _context.Vehiculos
            .Include(v => v.Usuario)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehiculo == null) return NotFound();

        var vm = VehiculoViewModel.FromModel(vehiculo);
        return View(vm);
    }

    // POST: Vehiculos/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehiculo = await _context.Vehiculos.FindAsync(id);
        if (vehiculo == null) return NotFound();

        if (await _context.Reservas.AnyAsync(r => r.VehiculoId == id && r.Estado == EstadoReserva.Activa))
        {
            TempData["ErrorMessage"] = "No se puede eliminar un vehículo con reservas activas.";
            return RedirectToAction(nameof(MisVehiculos));
        }

        try
        {
            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vehículo eliminado correctamente 🗑️";
            return RedirectToAction(nameof(MisVehiculos));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error al eliminar vehículo: {ex.Message}";
            return RedirectToAction(nameof(MisVehiculos));
        }
    }

    // GET: Vehiculos/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var vehiculo = await _context.Vehiculos
            .Include(v => v.Usuario) // dueño
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehiculo == null) return NotFound();

        var viewModel = new VehiculoViewModel
        {
            Id = vehiculo.Id,
            Marca = vehiculo.Marca,
            Modelo = vehiculo.Modelo,
            Placa = vehiculo.Placa,
            TarifaPorHora = vehiculo.TarifaPorHora,
            Disponible = vehiculo.Disponible,
            UsuarioNombre = vehiculo.Usuario?.Nombre
        };

        return View(viewModel);
    }



    // POST: Vehiculos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Vehiculo vehiculo)
    {
        if (id != vehiculo.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(vehiculo);

        try
        {
            _context.Update(vehiculo);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Vehículo actualizado correctamente.";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!VehiculoExists(vehiculo.Id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    private bool VehiculoExists(int id)
    {
        return _context.Vehiculos.Any(e => e.Id == id);
    }

}
