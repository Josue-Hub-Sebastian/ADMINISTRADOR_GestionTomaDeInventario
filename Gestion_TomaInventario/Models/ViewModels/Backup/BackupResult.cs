namespace Gestion_TomaInventario.Models.ViewModels.Backup
{
    public class BackupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ArchivoGenerado { get; set; } = string.Empty;
        public decimal PesoMB { get; set;}
        // solamente esto se creara para que se comunique con el backup service y pueda retornar el resultado de la operacion de backup
        //no toca sql 
    }
}
