namespace Gestion_TomaInventario.Models.ViewModels
{
    public class LicenciaViewModel
    {
        public long IdEmpresa { get; set; }
        public string? Nombre { get; set; }
        public string? Ruc { get; set; }
        public string? NombreBD { get; set; }
        public int AlmacenesMax { get; set; }
        public int UbicacionesMax { get; set; }
        public int ProductosMax { get; set; }
        public int UsuariosAdminMax { get; set; }
        public int UsuariosOperadorMax { get; set; }
        public int InventariosPreparadosMax { get; set; }
        public bool Estado { get; set; }

         // nuevos campos para almacenar la fecha :c
        public DateTime? InicioSuscripcion { get; set; }
        public DateTime? FinSuscripcion { get; set; }
        public int? MesesContratados { get; set; }


        // NUEVO: datos del plan asignado
        public int? IdPlan { get; set; }
        public bool EsPersonalizado { get; set; }

        public List<PlanDropdownViewModel> ListaPlanes { get; set; } = new();
    }
}
