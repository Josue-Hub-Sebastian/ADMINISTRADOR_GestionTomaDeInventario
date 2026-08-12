using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gestion_TomaInventario.Services.EmpresaServ;
using Gestion_TomaInventario.Models.ViewModels;
using Microsoft.Extensions.Logging;
using Gestion_TomaInventario.Services.PlanServ;

namespace Gestion_TomaInventario.Controllers
{
    [Authorize]
    public class EmpresaController : Controller
    {
        private readonly IEmpresaService _empresaService;
        private readonly ILogger<EmpresaController> _logger;
        private readonly IPlanService _planService;

        public EmpresaController(IEmpresaService empresaService, ILogger<EmpresaController> logger, IPlanService planService)
        {
            _empresaService = empresaService;
            _planService = planService;
            _logger = logger;
        }

        public async Task<IActionResult> Licencias()
        {
            var licencias = await _empresaService.ListarLicenciasAsync();
            return View(licencias);
        }

        // getter para obtener id
        [HttpGet]
        public async Task<IActionResult> Editar(long id)
        {
            _logger.LogDebug("Editar GET llamado con id={Id}", id);

            try
            {
                var licencia = await _empresaService.ObtenerLicenciaAsync((int)id);
                if (licencia == null)
                {
                    _logger.LogWarning("No se encontró Licencia para IdEmpresa={Id}", id);
                    return NotFound();
                }

                // 1. Obtener TODOS los planes (activos e inactivos) o al menos los activos + el asignado
                var planes = (await _empresaService.ListarPlanesActivosAsync()).ToList();

                // 2. Si la licencia tiene un plan asignado y NO está en la lista, agregarlo
                if (licencia.IdPlan.HasValue)
                {
                    var planYaEnLista = planes.Any(p => p.IdPlan == licencia.IdPlan.Value);

                    if (!planYaEnLista)
                    {
                        _logger.LogDebug("Plan {IdPlan} no está en activos, buscándolo individualmente...", licencia.IdPlan.Value);

                        var planAdicional = await _planService.ObtenerPlanAsync(licencia.IdPlan.Value);

                        if (planAdicional != null)
                        {
                            var planDropdown = new PlanDropdownViewModel
                            {
                                IdPlan = planAdicional.IdPlan,
                                NombrePlan = planAdicional.NombrePlan,
                                Almacenes = planAdicional.Almacenes.HasValue ? (int?)planAdicional.Almacenes.Value : null,
                                Ubicaciones = planAdicional.Ubicaciones.HasValue ? (int?)planAdicional.Ubicaciones.Value : null,
                                ProductosSkus = planAdicional.ProductosSkus.HasValue ? (int?)planAdicional.ProductosSkus.Value : null,
                                UsuariosAnd = planAdicional.UsuariosAnd.HasValue ? (int?)planAdicional.UsuariosAnd.Value : null,
                                UsuariosWeb = planAdicional.UsuariosWeb.HasValue ? (int?)planAdicional.UsuariosWeb.Value : null,
                                InventariosPreparadosMax = planAdicional.InventariosPreparadosMax.HasValue ? (int?)planAdicional.InventariosPreparadosMax.Value : null,
                                PrecioFijo = planAdicional.PrecioFijo,
                                PrecioLanzamiento = planAdicional.PrecioLanzamiento,
                                Estado = planAdicional.Estado
                            };
                            planes.Insert(0, planDropdown); // Insertar al inicio para que aparezca primero
                            _logger.LogDebug("Plan {IdPlan} ({Nombre}) agregado a la lista",
                                planAdicional.IdPlan, planAdicional.NombrePlan);
                        }
                        else
                        {
                            _logger.LogWarning("No se pudo obtener el plan {IdPlan} desde la BD", licencia.IdPlan.Value);
                        }
                    }
                    else
                    {
                        _logger.LogDebug("Plan {IdPlan} ya está en la lista de activos", licencia.IdPlan.Value);
                    }
                }

                licencia.ListaPlanes = planes;

                _logger.LogDebug("Licencia encontrada: {Nombre}, Plan actual: {IdPlan}, Planes en lista: {Count}",
                    licencia.Nombre, licencia.IdPlan, planes.Count);

                return View(licencia);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener licencia para IdEmpresa={Id}", id);
                throw;
            }
        }

        // post para emviar la nueva actualizacion de la licencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(LicenciaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errores = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { message = "ModelState inválido: " + errores });

                ModelState.AddModelError(string.Empty, "Datos inválidos: " + errores);
                return View(model);
            }

            try
            {
                var ok = await _empresaService.ActualizarLicenciaAsync(model);
                if (!ok)
                {
                    if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                        return BadRequest(new { message = "No se pudo actualizar la licencia en la BD." });

                    ModelState.AddModelError(string.Empty, "No se pudo actualizar la licencia.");
                    return View(model);
                }

                TempData["Success"] = "Licencia actualizada correctamente";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Ok(new { message = "OK" });

                return RedirectToAction(nameof(Licencias));
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return StatusCode(500, new { message = ex.Message });
                throw;
            }
        }


        public async Task<IActionResult> Dashboard(int idEmpresa)
        {
 //           _logger.LogDebug("Dashboard llamado con idEmpresa: {IdEmpresa}", idEmpresa);

            var modelo = await _empresaService.ObtenerDashboardEmpresaAsync(idEmpresa);


            if (modelo == null)
            {
                //_logger.LogWarning("No se encontraron datos para idEmpresa: {IdEmpresa}", idEmpresa);
                return NotFound();
            }
            modelo.Contacto = await _empresaService.ObtenerContactoEmpresaAsync(idEmpresa);
            modelo.Contacto ??= new ContactoEmpresaViewModel
            {
                IdEmpresa = idEmpresa,
                NombreEmpresa = modelo.NombreEmpresa
            };

            //_logger.LogDebug("Datos cargados: {NombreEmpresa}", modelo.NombreEmpresa);
            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> GuardarContacto(ContactoEmpresaViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest();

            var ok = await _empresaService.GuardarContactoEmpresaAsync(model);

            if (!ok) return BadRequest( new { message = "No se puede guardar el contacto . "});

            return Ok(new { message = "Contacto guardado correctamente ." });
        }




    }
}