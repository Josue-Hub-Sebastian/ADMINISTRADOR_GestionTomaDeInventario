using Gestion_TomaInventario.Models;
using Gestion_TomaInventario.Models.ViewModels;

namespace Gestion_TomaInventario.Repository.PlanRepo
{
    public interface IPlanRepository
    {
        Task<IReadOnlyList<PlanViewModel>> ListarPlanesAsync(bool soloActivos);
        Task<PlanFormViewModel?> ObtenerPlanAsync(int idPlan);
        Task<ResultadoOperacion> CrearPlanAsync(PlanFormViewModel model);
        Task<ResultadoOperacion> ActualizarPlanAsync(PlanFormViewModel model);
        Task<ResultadoOperacion> CambiarEstadoPlanAsync(int idPlan, bool estado);
    }
}
