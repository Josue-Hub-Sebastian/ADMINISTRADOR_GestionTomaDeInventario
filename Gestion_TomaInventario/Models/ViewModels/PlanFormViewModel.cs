namespace Gestion_TomaInventario.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class PlanFormViewModel
    {
        public int IdPlan { get; set; }

        [Required(ErrorMessage = "El nombre del plan es Obligatorio")]
        [Display(Name = "Nombre del plan")]
        public string NombrePlan { get; set; } = string.Empty;

        [Display(Name = "Almacenes")]
        public long? Almacenes { get; set; }

        [Display(Name = "Ubicaciones")]
        public long? Ubicaciones { get; set; }

        [Display(Name = "Productos / SKUs")]
        public long? ProductosSkus { get; set; }

        [Display(Name = "Usuarios App")]
        public long? UsuariosAnd { get; set; }

        [Display(Name = "Usuarios Web")]
        public long? UsuariosWeb { get; set; }

        [Display(Name = "Inventarios preparados")]
        public long? InventariosPreparadosMax { get; set; }

        [Required(ErrorMessage = "El precio fijo es obligatorio")]
        [Range(0, 999999, ErrorMessage = "El preio fijo debe ser mayor o igual a 0")]
        [Display(Name = "Precio fijo")]
        public decimal PrecioFijo { get; set; }

        [Display(Name = "Precio lanzamiento")]
        public decimal? PrecioLanzamiento { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; } = true;
    }
}
