using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Repository.EmpresaRepo;

namespace Gestion_TomaInventario.Services.EmpresaServ
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;

        public EmpresaService(IEmpresaRepository empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public Task<bool> ActualizarLicenciaAsync(LicenciaViewModel licencia) => _empresaRepository.ActualizarLicenciaAsync(licencia);
        

        public Task<IReadOnlyList<LicenciaViewModel>> ListarLicenciasAsync()
        {
            return _empresaRepository.ListarLicenciasAsync();
        }

        public Task<LicenciaViewModel> ObtenerLicenciaAsync(int id) =>  _empresaRepository.ObtenerLicenciaAsync(id);

        public Task<IReadOnlyList<EmpresaSincronizadaViewModel>> SincronizarEmpresasAsync()
        {
            return _empresaRepository.SincronizarEmpresasAsync();
        }

        public Task<DashboardEmpresaViewModel> ObtenerDashboardEmpresaAsync(int idEmpresa)
        {
            return _empresaRepository.ObtenerDashboardEmpresaAsync(idEmpresa);
        }

        public Task<IReadOnlyList<PlanDropdownViewModel>> ListarPlanesActivosAsync()
        {
            return _empresaRepository.ListarPlanesActivosAsync();
        }
    }
}
