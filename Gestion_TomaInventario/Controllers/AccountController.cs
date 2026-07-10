using System.Security.Claims;
using Gestion_TomaInventario.Models.Log;
using Gestion_TomaInventario.Services.UsuarioServ;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_TomaInventario.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUsuarioService usuarioService, ILogger<AccountController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocal(returnUrl);
            }

            return View(new Login { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var usuario = await _usuarioService.ValidarLoginAsync(model.Usuario.Trim(), model.Contrasena);
                var contrasenaValida = await _usuarioService.ValidarLoginAsync(model.Usuario.Trim(), model.Contrasena);

                if (usuario is null ||contrasenaValida is null)
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contrasena incorrectos.");
                    return View(model);
                }
                //ras tas tas
                

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                    new(ClaimTypes.Name, usuario.Usuario),
                    new("NombreCompleto", usuario.NombreCompleto ?? usuario.Usuario),
                    new("IdEmpresa", usuario.IdEmpresa?.ToString() ?? "0"),
                    new("EsSuperAdmin", usuario.EsSuperAdmin ? "true" : "false")
                };

                if (usuario.EsSuperAdmin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "SuperAdmin"));
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                        AllowRefresh = true
                    });

                return RedirectToLocal(model.ReturnUrl);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Configuracion incompleta para login.");
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Bienvenida", "Home");
        }
    }
}
