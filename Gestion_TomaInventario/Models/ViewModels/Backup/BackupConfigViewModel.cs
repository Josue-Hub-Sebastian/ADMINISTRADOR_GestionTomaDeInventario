namespace Gestion_TomaInventario.Models.ViewModels.Backup
{
    public class BackupConfigViewModel
    {
        public int IdBackupConfig { get; set; }
        public long IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string RutaDestino { get; set; } = string.Empty;
        public string Frecuencia { get; set; } = string.Empty;
        public byte? DiaSemana { get; set; }
        public byte? DiaMes { get; set; }
        public TimeSpan Hora { get; set; }
        public int MaxRespaldos { get; set; }
        public bool Comprimir { get; set; }
        public bool Activo { get; set; }
    }
}
