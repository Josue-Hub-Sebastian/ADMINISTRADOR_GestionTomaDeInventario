using Gestion_TomaInventario.Models.ViewModels;

namespace Gestion_TomaInventario.Services.EmpresaServ
{
    public interface IEmpresaService
    {
        Task<IReadOnlyList<EmpresaSincronizadaViewModel>> SincronizarEmpresasAsync();
        Task<IReadOnlyList<LicenciaViewModel>> ListarLicenciasAsync();

        //Metodos para obtener la licencia y actualizar la licencia de una empresa
        Task<LicenciaViewModel> ObtenerLicenciaAsync(int id); 
        Task<bool> ActualizarLicenciaAsync( LicenciaViewModel licencia); // int empresaId,  deberia pasar el id 
        Task<DashboardEmpresaViewModel> ObtenerDashboardEmpresaAsync(int idEmpresa);


        //canelita  
        Task<IReadOnlyList<PlanDropdownViewModel>> ListarPlanesActivosAsync();
    }
}
