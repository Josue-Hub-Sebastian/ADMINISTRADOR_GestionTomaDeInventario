using System.ComponentModel.DataAnnotations;

namespace Gestion_TomaInventario.Models.Log
{
    public class Login
    {
        [Required(ErrorMessage = "Ingrese el usuario.")]
        [StringLength(50)]
        public string Usuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la contrasena.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
