using Gestion_TomaInventario.Models.ViewModels.Backup;

namespace Gestion_TomaInventario.Services.BackUpManager
{
    public interface IBackupManager
    {
        // esta clase se conectara para ejecutar lo del backup
        Task<BackupResult> EjecutarBackupAsync(long idEmpresa);
        Task LimpiarBackupsAntiguosAsync(long idEmpresa);
    }
}
