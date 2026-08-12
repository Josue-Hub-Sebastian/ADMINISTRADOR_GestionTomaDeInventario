using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Models.ViewModels.Backup;
using Gestion_TomaInventario.Repository.BackUpRepo;
using Gestion_TomaInventario.Services.BackUpManager;

namespace Gestion_TomaInventario.Services.BackupServ
{
    public class BackupService : IBackupService
    {

        private readonly IBackupRepository _backupRepository;
        private readonly IBackupManager _backupManager;
        public BackupService(IBackupRepository backupRepository, IBackupManager backupManager)
        {
            _backupRepository = backupRepository;
            _backupManager = backupManager;
        }

        public async Task<bool> GuardarConfiguracionBackupAsync(BackupConfigViewModel model)=>await _backupRepository.GuardarConfiguracionBackupAsync(model);

        public async Task<IReadOnlyList<BackupHistorialViewModel>> ListarHistorialBackupsAsync(long idEmpresa)=> await _backupRepository.ListarHistorialBackupsAsync(idEmpresa);

        public Task<BackupConfigViewModel> ObtenerConfiguracionBackupAsync(long idEmpresa) => _backupRepository.ObtenerConfiguracionBackupAsync(idEmpresa);

        // Task<int> RegistrarHistorialAsync(BackupHistorialViewModel model);


        // Recordar que el service no hace el backup solo se invoca , la logica esta en IBackupManager
        public Task<BackupResult> RealizarBackupAsync(long idEmpresa)=> _backupManager.EjecutarBackupAsync(idEmpresa);

        public Task<InstanciaClienteViewModel?> ObtenerInstanciaClienteAsync(long idEmpresa)=> _backupRepository.ObtenerInstanciaClienteAsync(idEmpresa);

        public async Task<IReadOnlyList<EmpresaBackupResumenViewModel>> ListarBackupResumenAsync()=> await _backupRepository.ListarEmpresasParaBackupAsync();
        

       
    }
}
