using System.Diagnostics;
using Gestion_TomaInventario.Models;
using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Services.EmpresaServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_TomaInventario.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmpresaService _empresaService;

        public HomeController(ILogger<HomeController> logger, IEmpresaService empresaService)
        {
            _logger = logger;
            _empresaService = empresaService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            try
            {
                var empresas = await _empresaService.SincronizarEmpresasAsync();
                return View(empresas);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "No se pudo sincronizar empresas por configuracion incompleta.");
                ViewBag.ErrorSincronizacion = ex.Message;
                return View(Array.Empty<EmpresaSincronizadaViewModel>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar SP_SincronizarEmpresas.");
                ViewBag.ErrorSincronizacion = "No se pudo ejecutar SP_SincronizarEmpresas. Revise la conexion y el procedimiento almacenado.";
                return View(Array.Empty<EmpresaSincronizadaViewModel>());
            }
        }

        // Nueva acción: muestra Views/Home/Bienvenida.cshtml
        [Authorize]
        public IActionResult Bienvenida()
        {
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
