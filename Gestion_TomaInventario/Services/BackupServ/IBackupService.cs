using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Models.ViewModels.Backup;

namespace Gestion_TomaInventario.Services.BackupServ
{
    public interface IBackupService
    {
        Task<BackupConfigViewModel> ObtenerConfiguracionBackupAsync(long idEmpresa);
        Task<bool> GuardarConfiguracionBackupAsync(BackupConfigViewModel model);
        Task<IReadOnlyList<BackupHistorialViewModel>> ListarHistorialBackupsAsync(long idEmpresa);
        Task<BackupResult> RealizarBackupAsync(long idEmpresa);
        Task<InstanciaClienteViewModel?> ObtenerInstanciaClienteAsync(long idEmpresa);
        Task<IReadOnlyList<EmpresaBackupResumenViewModel>> ListarBackupResumenAsync();

    }
}
