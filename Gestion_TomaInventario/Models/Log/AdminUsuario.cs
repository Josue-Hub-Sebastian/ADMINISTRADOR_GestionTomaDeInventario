using System.ComponentModel.DataAnnotations.Schema;

namespace Gestion_TomaInventario.Models.Log
{
    [Table("AdminUsuario_GT")]
    public class AdminUsuario
    {
        public long IdUsuario { get; set; }
        public long? IdEmpresa { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? NombreCompleto { get; set; }
        public bool EsSuperAdmin { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaRegistro { get; set; }

    }
}
