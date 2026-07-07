using Gestion_TomaInventario.Models.Log;
using Gestion_TomaInventario.Repository.UsuarioRepo;

namespace Gestion_TomaInventario.Services.UsuarioServ
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public Task<AdminUsuario?> ValidarLoginAsync(string usuario, string contrasena)
        {
            return _usuarioRepository.ValidarLoginAsync(usuario, contrasena);
        }
    }
}
