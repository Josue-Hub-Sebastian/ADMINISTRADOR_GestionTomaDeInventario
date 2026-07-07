namespace Gestion_TomaInventario.Models.ViewModels
{
    public class PlanViewModel
    {
        public int IdPlan { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public int? Almacenes { get; set; }
        public int? Ubicaciones { get; set; }
        public int? ProductosSkus { get; set; } // eran long pero se cambio a int para que no de error en la base de datos
        public int? UsuariosAnd { get; set; }
        public int? UsuariosWeb { get; set; }
        public int? InventariosPreparadosMax { get; set; }
        public decimal PrecioFijo { get; set; }
        public decimal? PrecioLanzamiento { get; set; }
        public bool Estado { get; set; }
    }
}
