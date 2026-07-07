namespace Gestion_TomaInventario.Models.ViewModels
{
    public class EmpresaSincronizadaViewModel
    {
        public long IdEmpresa { get; set; }
        public string? CodEmpresa { get; set; }
        public string? Ruc { get; set; }
        public string? Nombre { get; set; }
        public string? NombreBD { get; set; }
        // agregado para manejar los datos de editar :v
        public bool Estado { get; set; }
    }
}
