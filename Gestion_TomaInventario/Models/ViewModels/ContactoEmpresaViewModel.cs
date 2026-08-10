namespace Gestion_TomaInventario.Models.ViewModels
{
    public class ContactoEmpresaViewModel
    {
        public long IdContacto { get; set; }
        public long IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; } = "";
        public string NombreContacto { get; set; } = "";
        public string TelefonoContacto { get; set; } = "";
        public string? CorreoContacto { get; set; }
        public bool? Estado { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public DateTime? FechaActualizacion { get; set; }



    }
}
