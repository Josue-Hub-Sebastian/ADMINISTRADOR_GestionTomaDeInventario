using Gestion_TomaInventario.Models.Log;

namespace Gestion_TomaInventario.Repository.UsuarioRepo
{
    public interface IUsuarioRepository
    {
        Task<AdminUsuario?> ValidarLoginAsync(string usuario, string contrasena);
    }
}
