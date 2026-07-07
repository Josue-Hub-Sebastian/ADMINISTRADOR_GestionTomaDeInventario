using Gestion_TomaInventario.Models.Log;

namespace Gestion_TomaInventario.Services.UsuarioServ
{
    public interface IUsuarioService
    {
        Task<AdminUsuario?> ValidarLoginAsync(string usuario, string contrasena);
    }
}
