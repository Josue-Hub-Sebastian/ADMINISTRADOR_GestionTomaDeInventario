namespace Gestion_TomaInventario.Models.ViewModels
{
    public class PlanDropdownViewModel
    {
        public int IdPlan { get; set; }
        public string NombrePlan { get; set; } = string.Empty;

        public int? Almacenes { get; set; }
        public int? Ubicaciones { get; set; }
        public int? ProductosSkus { get; set; }
        public int? UsuariosAnd { get; set; }
        public int? UsuariosWeb { get; set; }
        public int? InventariosPreparadosMax { get; set; }

        public decimal PrecioFijo { get; set; }
        public decimal? PrecioLanzamiento { get; set; }

        // estado 
        public bool Estado { get; set; }
    }
}
