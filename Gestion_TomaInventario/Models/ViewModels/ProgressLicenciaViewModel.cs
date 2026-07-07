namespace Gestion_TomaInventario.Models.ViewModels
{
    public class ProgressLicenciaViewModel
    {
        public string Titulo { get; set; } = string.Empty;
        public int Actual {  get; set; }
        public int Maximo { get; set; }
        public decimal Porcentaje { get; set; }
        public string Estado { get; set; } = string.Empty;// era bit
        public string ClaseBoopstrap { get; set; } = string.Empty;
    
    }
}
