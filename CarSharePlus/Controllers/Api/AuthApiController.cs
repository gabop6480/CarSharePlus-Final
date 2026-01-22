using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarSharePlus.Controllers.Api
{
    // CAMBIO IMPORTANTE: Forzamos la ruta a "api/auth" explícitamente
    [Route("api/auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // LOGS DE DEPURACIÓN (Míralos en la consola negra del servidor)
            Console.WriteLine($"[API LOGIN] Intento de: {model.Correo}");

            // 1. Validar que lleguen datos
            if (string.IsNullOrEmpty(model.Correo) || string.IsNullOrEmpty(model.Password))
            {
                Console.WriteLine("[API LOGIN] Fallo: Correo o Password llegaron vacíos (Problema de JSON).");
                return BadRequest(new { message = "Datos incompletos" });
            }

            // 2. Buscar usuario
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == model.Correo && u.Password == model.Password);

            if (usuario == null)
            {
                Console.WriteLine("[API LOGIN] Fallo: Usuario no encontrado o contraseña incorrecta en BD.");
                return Unauthorized(new { message = "Credenciales incorrectas" });
            }

            Console.WriteLine($"[API LOGIN] Éxito: Bienvenido {usuario.Nombre}");

            // 3. Crear cookie (opcional para API móvil, pero útil si compartes lógica)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Ok(usuario);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Sesión cerrada" });
        }
    }

    // Clase para recibir los datos
    public class LoginModel
    {
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}