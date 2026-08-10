namespace Gestion_TomaInventario.Models.ViewModels.Backup
{
    public class BackupHistorialViewModel
    {
        public long IdHistorial { get; set; }
        public long IdEmpresa { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? ArchivoGenerado { get; set; }
        public decimal? PesoMB { get; set; }
        public string? Estado { get; set; }
        public string? MensajeError { get; set; } 
    }
}
