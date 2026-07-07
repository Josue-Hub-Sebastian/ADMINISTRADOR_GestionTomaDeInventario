using System.Data;
using Gestion_TomaInventario.Models.Log;
using Gestion_TomaInventario.Repository.ProcedimientosAlmacenados;
using Microsoft.Data.SqlClient;

namespace Gestion_TomaInventario.Repository.UsuarioRepo
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IConfiguration _configuration;

        public UsuarioRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AdminUsuario?> ValidarLoginAsync(string usuario, string contrasena)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("No se encontro la cadena de conexion 'DefaultConnection'.");
            }

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(StoredProcedures.ValidarLoginAdmin, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = usuario;
                    cmd.Parameters.Add("@Contrasena", SqlDbType.VarChar, 255).Value = contrasena;

                    await cn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync())
                        {
                            return null;
                        }

                        return new AdminUsuario
                        {
                            IdUsuario = reader.GetInt64(reader.GetOrdinal("IdUsuario")),
                            IdEmpresa = reader.IsDBNull(reader.GetOrdinal("IdEmpresa")) ? null : reader.GetInt64(reader.GetOrdinal("IdEmpresa")),
                            Usuario = reader.GetString(reader.GetOrdinal("Usuario")),
                            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
                            NombreCompleto = reader.IsDBNull(reader.GetOrdinal("NombreCompleto")) ? null : reader.GetString(reader.GetOrdinal("NombreCompleto")),
                            EsSuperAdmin = reader.GetBoolean(reader.GetOrdinal("EsSuperAdmin")),
                            Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                            FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                        };
                    }
                }
            }
        }
    }
}
