using Gestion_TomaInventario.Models.ViewModels.Backup;
using Gestion_TomaInventario.Services.BackupServ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestion_TomaInventario.Controllers
{
    [Authorize]
    public class BackupController : Controller
    {
        private readonly IBackupService _backupService;

        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        public async Task<IActionResult> Index()
        {
            var empresas = await _backupService.ListarBackupResumenAsync();
            return View(empresas);
        }

        public async Task<IActionResult> Configurar(long idEmpresa)
        {
            BackupConfigViewModel? configuracion = await _backupService.ObtenerConfiguracionBackupAsync(idEmpresa);

            configuracion ??= new BackupConfigViewModel
            {
                IdEmpresa = idEmpresa,
                Frecuencia = "SEMANAL",
                MaxRespaldos = 10,
                Activo = true
            };

            return View(configuracion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarConfiguracion(BackupConfigViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los campos del formulario.";
                return View("Configurar", model);
            }

            bool guardado = await _backupService.GuardarConfiguracionBackupAsync(model);

            TempData[guardado ? "Success" : "Error"] = guardado
                ? "Configuración de backup guardada correctamente."
                : "No se pudo guardar la configuración.";

            return RedirectToAction(nameof(Configurar), new { idEmpresa = model.IdEmpresa });
        }

        public async Task<IActionResult> Historial(long idEmpresa)
        {
            var historial = await _backupService.ListarHistorialBackupsAsync(idEmpresa);
            ViewBag.IdEmpresa = idEmpresa; // para el boton "volver" y el titulo
            return View(historial);
        }

        // POST: /Backup/EjecutarManual  (idEmpresa viaja en el form, no en la ruta)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EjecutarManual(long idEmpresa)
        {
            BackupResult resultado = await _backupService.RealizarBackupAsync(idEmpresa);

            TempData[resultado.Success ? "Success" : "Error"] = resultado.Message;

            return RedirectToAction(nameof(Historial), new { idEmpresa });
        }
    }
}
