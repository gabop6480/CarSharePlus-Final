using CarSharePlus.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarSharePlusShared.Models;

[Route("api/[controller]")]
[ApiController]
public class PagosApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PagosApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/pagos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Pago>>> GetPagos()
    {
        return await _context.Pagos
            .Include(p => p.Reserva)
                .ThenInclude(r => r.Usuario)
            .Include(p => p.Reserva)
                .ThenInclude(r => r.Vehiculo)
            .ToListAsync();
    }

    // GET: api/pagos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Pago>> GetPago(int id)
    {
        var pago = await _context.Pagos
            .Include(p => p.Reserva)
                .ThenInclude(r => r.Usuario)
            .Include(p => p.Reserva)
                .ThenInclude(r => r.Vehiculo)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pago == null) return NotFound();
        return pago;
    }

    // POST: api/pagos
    [HttpPost]
    public async Task<ActionResult<Pago>> PostPago(Pago pago)
    {
        if (pago.Monto <= 0)
            return BadRequest(new { error = "El monto debe ser mayor a 0." });

        var reserva = await _context.Reservas.FindAsync(pago.ReservaId);
        if (reserva == null)
            return BadRequest(new { error = "La reserva asociada no existe." });

        var existePagoConfirmado = await _context.Pagos
            .AnyAsync(p => p.ReservaId == pago.ReservaId && p.Confirmado);
        if (existePagoConfirmado)
            return BadRequest(new { error = "Ya existe un pago confirmado para esta reserva." });

        _context.Pagos.Add(pago);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPago), new { id = pago.Id }, pago);
    }

    // PUT: api/pagos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPago(int id, Pago pago)
    {
        if (id != pago.Id) return BadRequest(new { error = "El ID no coincide." });

        if (pago.Monto <= 0)
            return BadRequest(new { error = "El monto debe ser mayor a 0." });

        _context.Entry(pago).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/pagos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePago(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago == null) return NotFound();

        if (pago.Confirmado)
            return BadRequest(new { error = "No se puede eliminar un pago confirmado." });

        _context.Pagos.Remove(pago);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PATCH: api/pagos/confirmar/5
    [HttpPatch("confirmar/{id}")]
    public async Task<IActionResult> ConfirmarPago(int id)
    {
        var pago = await _context.Pagos.FindAsync(id);
        if (pago == null) return NotFound();

        if (pago.Confirmado)
            return BadRequest(new { error = "El pago ya estaba confirmado." });

        pago.Confirmado = true;
        _context.Update(pago);
        await _context.SaveChangesAsync();

        return Ok(pago);
    }
}
