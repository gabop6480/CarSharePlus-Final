using BCrypt.Net;
using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlus.Controllers
{
    [Authorize] // obliga a estar autenticado para todo el controlador
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context) => _context = context;

        // GET: Usuarios (solo Admin puede ver todos)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Vehiculos)
                .AsNoTracking()
                .ToListAsync();

            return View(usuarios);
        }

        // GET: Usuarios/Details/5 (Admin puede ver cualquiera, usuario solo el suyo)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound("Debe especificar un ID de usuario.");

            var usuario = await _context.Usuarios
                .Include(u => u.Vehiculos)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null) return NotFound($"No se encontró el usuario con ID {id}.");

            // Si no es Admin, solo puede ver su propio perfil
            if (!User.IsInRole("Admin") && usuario.Correo != User.Identity?.Name)
                return Forbid();

            return View(usuario);
        }

        // GET: Usuarios/Create (solo Admin)
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // POST: Usuarios/Create (solo Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Nombre,Correo,Telefono,Rol,Password")] Usuario usuario)
        {
            if (!ModelState.IsValid) return View(usuario);

            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo))
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                return View(usuario);
            }

            // Guardar la contraseña tal cual la ingresó el usuario
            _context.Add(usuario);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Usuario creado correctamente ✅";
            return RedirectToAction(nameof(Index));
        }


        // GET: Usuarios/Edit/5 (solo Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound("Debe especificar un ID de usuario.");

            var usuario = await _context.Usuarios
                .Include(u => u.Vehiculos)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound($"No se encontró el usuario con ID {id}.");

            return View(usuario);
        }

        // POST: Usuarios/Edit/5 (solo Admin)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo,Telefono,Rol")] Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest("El ID no coincide con el usuario a editar.");
            if (!ModelState.IsValid) return View(usuario);

            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo && u.Id != usuario.Id))
            {
                ModelState.AddModelError("Correo", "El correo ya está registrado.");
                return View(usuario);
            }

            var original = await _context.Usuarios.Include(u => u.Vehiculos).FirstOrDefaultAsync(u => u.Id == id);
            if (original == null) return NotFound($"No se encontró el usuario con ID {id}.");

            try
            {
                original.Nombre = usuario.Nombre;
                original.Correo = usuario.Correo;
                original.Telefono = usuario.Telefono;
                original.Rol = usuario.Rol;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario actualizado correctamente ✏️";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["ErrorMessage"] = "Conflicto de concurrencia al actualizar el usuario.";
                return View(usuario);
            }
        }

        // GET: Usuarios/Delete/5 (solo Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound("Debe especificar un ID de usuario.");

            var usuario = await _context.Usuarios
                .Include(u => u.Vehiculos)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (usuario == null) return NotFound($"No se encontró el usuario con ID {id}.");

            return View(usuario);
        }

        // POST: Usuarios/Delete/5 (solo Admin)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.Include(u => u.Vehiculos).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound($"No se encontró el usuario con ID {id}.");

            if (usuario.Vehiculos.Any())
            {
                TempData["ErrorMessage"] = "No se puede eliminar un usuario con vehículos asociados.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Usuario eliminado correctamente 🗑️";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar usuario: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
