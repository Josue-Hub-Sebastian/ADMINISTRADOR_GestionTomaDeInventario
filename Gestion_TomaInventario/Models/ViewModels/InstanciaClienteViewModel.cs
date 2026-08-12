namespace Gestion_TomaInventario.Models.ViewModels
{
    public class InstanciaClienteViewModel
    {
        public int IdInstancia { get; set; }
        public long IdEmpresa { get; set; }
        public string NombreBaseDatos { get; set; } = string.Empty;
        public string NombreServidor { get; set; } = string.Empty;
      //   public string UsuarioSql { get; set; }
      //  public string PasswordSql { get; set; }
        public bool Estado { get; set; }
    }
}
