using CarSharePlus.Data;
using CarSharePlusShared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarSharePlus.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login() => View();

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Password == password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Correo o contraseña incorrectos.");
                return View();
            }

            // Claims: correo como Name y rol como Role
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),   // 🔥 esto se usa en VehiculosController
                new Claim(ClaimTypes.Role, usuario.Rol)       // rol para Authorize(Roles="...")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Opcional: log para depuración
            Console.WriteLine($"Login exitoso: {usuario.Correo} con rol {usuario.Rol}");

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: Account/Register
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (!ModelState.IsValid) 
                return View(usuario); 
            if (await _context.Usuarios.AnyAsync(u => u.Correo == usuario.Correo)) 
            { 
                ModelState.AddModelError("Correo", "El correo ya está en uso."); return View(usuario); 
            }
            if (await _context.Usuarios.AnyAsync(u => u.Telefono == usuario.Telefono))
            {
                ModelState.AddModelError("Telefono", "El telefono ya está en uso."); return View(usuario);
            }

            usuario.Rol = "User";
            usuario.Password = usuario.Password;
            usuario.Telefono = usuario.Telefono;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Opcional: iniciar sesión automáticamente tras registrarse
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData["SuccessMessage"] = "Cuenta creada correctamente ✅"; 
            return RedirectToAction("Index", "Home");
        }
        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword() => View();
        // POST: Account/ForgotPassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string correo)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "No se encontró una cuenta con ese correo.";
                return View();
            }

            // Redirige al formulario de cambio de contraseña
            return RedirectToAction("ResetPassword", new { correo = usuario.Correo });
        }
        // GET: Account/ResetPassword

        public IActionResult ResetPassword(string correo)
        {
            ViewBag.Correo = correo;
            return View();
        }
        // POST: Account/ResetPassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string correo, string nuevaPassword)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "No se encontró el usuario.";
                return RedirectToAction("ForgotPassword");
            }

            usuario.Password = nuevaPassword;
            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña actualizada correctamente ✅";
            return RedirectToAction("Login");
        }


    }
}
