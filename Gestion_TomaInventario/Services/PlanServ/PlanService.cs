using Gestion_TomaInventario.Models;
using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Repository.PlanRepo;

namespace Gestion_TomaInventario.Services.PlanServ
{
    public class PlanService : IPlanService
    {


        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }


        public Task<IReadOnlyList<PlanViewModel>> ListarPlanesAsync(bool soloActivos) => _planRepository.ListarPlanesAsync(soloActivos);

        public Task<PlanFormViewModel?> ObtenerPlanAsync(int idPlan) => _planRepository.ObtenerPlanAsync(idPlan);

        public Task<ResultadoOperacion> CrearPlanAsync(PlanFormViewModel model) => _planRepository.CrearPlanAsync(model);

        public Task<ResultadoOperacion> ActualizarPlanAsync(PlanFormViewModel model) => _planRepository.ActualizarPlanAsync(model);

        public Task<ResultadoOperacion> CambiarEstadoPlanAsync(int idPlan, bool estado) => _planRepository.CambiarEstadoPlanAsync(idPlan, estado);
    }
}
