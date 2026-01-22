using CarSharePlus.Data;   // Tu DbContext
using CarSharePlusShared.Models; // Tu modelo Usuario
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarSharePlus.Controllers.Api
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------------------
        // POST: /api/usuarios/login
        // -------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.Password == dto.Password);

            if (usuario == null)
                return Unauthorized("Credenciales inválidas");

            // Simulamos un token (puedes devolver un GUID si quieres)
            var token = Guid.NewGuid().ToString();

            return Ok(new
            {
                Token = token,
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol
            });
        }

        // -------------------------------
        // GET: /api/usuarios/perfil?id=5
        // -------------------------------
        [HttpGet("perfil")]
        public async Task<ActionResult<Usuario>> GetPerfil([FromQuery] int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            return usuario;
        }

        // -------------------------------
        // PUT: /api/usuarios/perfil
        // -------------------------------
        [HttpPut("perfil")]
        public async Task<IActionResult> ActualizarPerfil([FromBody] Usuario usuario)
        {
            var existente = await _context.Usuarios.FindAsync(usuario.Id);
            if (existente == null)
                return NotFound();

            existente.Nombre = usuario.Nombre;
            existente.Correo = usuario.Correo;
            existente.Telefono = usuario.Telefono;
            existente.Password = usuario.Password;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // DTO para login
    public class LoginDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
