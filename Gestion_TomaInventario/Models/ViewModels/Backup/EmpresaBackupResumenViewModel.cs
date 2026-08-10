namespace Gestion_TomaInventario.Models.ViewModels.Backup
{
    public class EmpresaBackupResumenViewModel
    {
        public long IdEmpresa { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public bool EstadoEmpresa { get; set; }
        public long IdInstancia { get; set; }
        public string NombreBD { get; set; } = string.Empty;
        public string ServidorSql { get; set; } = string.Empty;
        public bool EstadoInstancia { get; set; }
        public DateTime? UltimoBackupFecha { get; set; }
        public string? UltimoBackupEstado { get; set; }
    }
}