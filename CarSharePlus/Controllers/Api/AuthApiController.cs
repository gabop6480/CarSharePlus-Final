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
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == model.Correo && u.Password == model.Password);

            if (usuario == null)
                return Unauthorized(new { message = "Credenciales incorrectas" });

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Esto crea la cookie que HttpClient guardará automáticamente
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

    public class LoginModel
    {
        public string Correo { get; set; }
        public string Password { get; set; }
    }
}