using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarSharePlus.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login([FromBody] LoginRequest request)
        {
            // 1. Imprimir lo que llega (PARA DEPURAR)
            Console.WriteLine($"[LOGIN INTENTO] Correo: '{request.Correo}' - Password recibido: '{request.Password}'");

            // 2. Buscar usuario solo por correo primero
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (usuario == null)
            {
                Console.WriteLine("[LOGIN FALLO] El usuario no existe en la BD.");
                return Unauthorized("Usuario no encontrado.");
            }

            // 3. Imprimir lo que hay en la BD (PARA DEPURAR)
            Console.WriteLine($"[LOGIN BD] Usuario encontrado. Password en BD: '{usuario.Password}'");

            // 4. Comparación DIRECTA (Texto plano) - Úsalo para probar si esto falla
            // Si usas Hash, aquí deberías usar BCrypt.Verify o similar.
            if (usuario.Password != request.Password)
            {
                Console.WriteLine("[LOGIN FALLO] La contraseña no coincide.");
                return Unauthorized("Contraseña incorrecta.");
            }

            // 5. Login exitoso
            Console.WriteLine("[LOGIN EXITOSO] Credenciales válidas.");

            // Evitamos devolver la contraseña al cliente por seguridad
            usuario.Password = "";
            return Ok(usuario);
        }

        // Clase auxiliar para recibir los datos (puedes ponerla dentro del mismo archivo o namespace)
        public class LoginRequest
        {
            public string Correo { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { message = "Sesión cerrada" });
        }
    }

    public class LoginModel
    {
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}