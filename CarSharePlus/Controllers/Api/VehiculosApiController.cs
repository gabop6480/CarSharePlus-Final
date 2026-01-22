using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarSharePlus.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VehiculosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehiculosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/vehiculos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetVehiculos()
        {
            return await _context.Vehiculos
                .Include(v => v.Usuario)
                .ToListAsync();
        }

        // GET: api/vehiculos/mis-vehiculos
        [HttpGet("mis-vehiculos")]
        public async Task<ActionResult<IEnumerable<Vehiculo>>> GetMisVehiculos()
        {
            var correo = User.Identity.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null) return Unauthorized();

            return await _context.Vehiculos
                .Where(v => v.UsuarioId == usuario.Id)
                .ToListAsync();
        }

        // POST: api/vehiculos
        [HttpPost]
        public async Task<ActionResult<Vehiculo>> PostVehiculo(Vehiculo vehiculo)
        {
            var correo = User.Identity.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null) return Unauthorized();

            vehiculo.UsuarioId = usuario.Id; // Asignar dueño automáticamente

            if (await _context.Vehiculos.AnyAsync(v => v.Placa == vehiculo.Placa))
                return BadRequest("La placa ya existe.");

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVehiculos", new { id = vehiculo.Id }, vehiculo);
        }

        // DELETE: api/vehiculos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehiculo(int id)
        {
            var correo = User.Identity.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null) return Unauthorized();

            var vehiculo = await _context.Vehiculos
                .FirstOrDefaultAsync(v => v.Id == id && v.UsuarioId == usuario.Id);

            if (vehiculo == null) return NotFound();

            if (await _context.Reservas.AnyAsync(r => r.VehiculoId == id && r.Estado == EstadoReserva.Activa))
                return BadRequest("No se puede eliminar un vehículo con reservas activas.");

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}