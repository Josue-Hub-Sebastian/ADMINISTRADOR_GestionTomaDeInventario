using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Services.PlanServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_TomaInventario.Controllers
{
    [Authorize]
    public class PlanesController : Controller
    {
        private readonly IPlanService _planService;

        public PlanesController(IPlanService planService)
        {
            _planService = planService;
        }

        // vista principal de planes
        public async Task<IActionResult> Index()
        {
            var planes = await _planService.ListarPlanesAsync(false);
            return View(planes);
        }

        [HttpGet]
        public IActionResult Crear()
        {
            var model = new PlanFormViewModel
            {
                Estado = true // Establecer el estado predeterminado en activo
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(PlanFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _planService.CrearPlanAsync(model);

            if (resultado.Exito)
            {
                TempData["Success"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = resultado.Mensaje;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var plan = await _planService.ObtenerPlanAsync(id);
            if (plan == null)
            {
                TempData["Error"] = "No se encontro el plan solicitado";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(PlanFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _planService.ActualizarPlanAsync(model);

            if (resultado.Exito)
                {
                    TempData["Success"] = resultado.Mensaje;
                    return RedirectToAction(nameof(Index));
                }

            TempData["Error"] = resultado.Mensaje;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool estado)
        {
            var resultado = await _planService.CambiarEstadoPlanAsync(id, estado);

            if (resultado.Exito)
                TempData["Success"] = resultado.Mensaje;
            else
                TempData["Error"] = resultado.Mensaje;

            return RedirectToAction(nameof(Index));
        }
    }
}
