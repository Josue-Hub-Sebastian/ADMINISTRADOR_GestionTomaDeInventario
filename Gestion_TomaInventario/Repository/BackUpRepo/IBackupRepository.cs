using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Models.ViewModels.Backup;

namespace Gestion_TomaInventario.Repository.BackUpRepo
{
    public interface IBackupRepository
    {
        //no hace backup aqui ;c
        Task<BackupConfigViewModel> ObtenerConfiguracionBackupAsync(long idEmpresa);
        Task<bool> GuardarConfiguracionBackupAsync(BackupConfigViewModel model);
        Task<List<BackupHistorialViewModel>> ListarHistorialBackupsAsync(long idEmpresa); // solo trae uno xd antes era un Ireadonly xd
        Task<int> RegistrarHistorialAsync(BackupHistorialViewModel model);
        Task<InstanciaClienteViewModel?> ObtenerInstanciaClienteAsync(long idEmpresa);

        Task MarcarHistoriaEliminadoAsync(long idHistorial);
        Task<List<EmpresaBackupResumenViewModel>> ListarEmpresasParaBackupAsync();
        //Task GuardarConfiguracionBackupAsync();


    
    }
}
