using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlus.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EvaluacionesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EvaluacionesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/evaluaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evaluacion>>> GetEvaluaciones()
        {
            return await _context.Evaluaciones
                .Include(e => e.Usuario)
                .Include(e => e.Vehiculo)
                .OrderByDescending(e => e.Id)
                .ToListAsync();
        }

        // POST: api/evaluaciones
        [HttpPost]
        public async Task<ActionResult<Evaluacion>> PostEvaluacion(Evaluacion evaluacion)
        {
            var correo = User.Identity?.Name;
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null) return Unauthorized();

            evaluacion.UsuarioId = usuario.Id; // Asignar autor automáticamente

            // Validar si el vehículo existe
            if (!await _context.Vehiculos.AnyAsync(v => v.Id == evaluacion.VehiculoId))
                return BadRequest("El vehículo no existe.");

            _context.Evaluaciones.Add(evaluacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvaluaciones), new { id = evaluacion.Id }, evaluacion);
        }
    }
}