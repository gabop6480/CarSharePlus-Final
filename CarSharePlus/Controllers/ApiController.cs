using System.Runtime.InteropServices;
using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlus.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reservas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetReservas()
        {
            return await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .Include(r => r.Pagos)
                .ToListAsync();
        }

        // GET: api/reservas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reserva>> GetReserva(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .Include(r => r.Vehiculo)
                .Include(r => r.Pagos)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound();
            return reserva;
        }

        // POST: api/reservas
        [HttpPost]
        public async Task<ActionResult<Reserva>> PostReserva(Reserva reserva)
        {
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReserva), new { id = reserva.Id }, reserva);
        }

        // PUT: api/reservas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReserva(int id, Reserva reserva)
        {
            if (id != reserva.Id) return BadRequest();

            _context.Entry(reserva).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/reservas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReserva(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null) return NotFound();

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
