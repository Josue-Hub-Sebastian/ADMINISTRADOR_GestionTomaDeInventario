using Gestion_TomaInventario.Models.ViewModels;

namespace Gestion_TomaInventario.Repository.EmpresaRepo
{
    public interface IEmpresaRepository
    {
        Task<IReadOnlyList<EmpresaSincronizadaViewModel>> SincronizarEmpresasAsync();
        Task<IReadOnlyList<LicenciaViewModel>> ListarLicenciasAsync();
        Task<LicenciaViewModel> ObtenerLicenciaAsync(int id);
        Task<bool> ActualizarLicenciaAsync(LicenciaViewModel licencia);
        Task<DashboardEmpresaViewModel> ObtenerDashboardEmpresaAsync(int idEmpresa);


        // usamos como dto para el combo box de planes activos, ya que no necesitamos toda la información de la entidad Plan
        //canelita: 2024-06-19: Se agrega el método ListarPlanesActivosAsync para obtener los planes activos de la empresa, usando el ViewModel PlanDropdownViewModel como DTO para el combo box.
        Task<IReadOnlyList<PlanDropdownViewModel>> ListarPlanesActivosAsync();


        //contacto
        Task<ContactoEmpresaViewModel?> ObtenerContactoEmpresaAsync(int idEmpresa);
        Task<bool> GuardarContactoEmpresa(ContactoEmpresaViewModel model);
    }
}
