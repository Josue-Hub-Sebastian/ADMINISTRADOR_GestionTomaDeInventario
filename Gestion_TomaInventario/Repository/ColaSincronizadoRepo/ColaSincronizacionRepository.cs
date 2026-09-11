
using Microsoft.Data.SqlClient;
using System.Data;

namespace Gestion_TomaInventario.Repository.ColaSincronizadoRepo
{
    public class ColaSincronizacionRepository : IColaSincronizacionRepository
    {

        private readonly IConfiguration _configuration;

        public ColaSincronizacionRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Falta 'DefaultConnection' en appsetings.json");

        public async Task MarcarErrorAsync(long idEmpresa, string error)
        {
            var msg = error.Length > 500 ? error.Substring(0, 500) :error;

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE dbo.ColaSincronizacion_GT " +
                "SET Intentos = Intentos + 1, UltimoError = @Error " +
                "WHERE IdEmpresa = @IdEmpresa AND Procesado = 0", cn);
            cmd.Parameters.Add("@Error", SqlDbType.NVarChar,500).Value = msg;
            cmd.Parameters.Add("@IdEmpresa", SqlDbType.BigInt).Value = idEmpresa;

            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task MarcarProcesadoAsync(long idEmpresa)
        {
           using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("UPDATE dbo.ColaSincronizacion_GT " +
                 "SET Procesado = 1, FechaProcesado = GETDATE() " +
                 "WHERE IdEmpresa = @IdEmpresa AND Procesado = 0", cn);
            cmd.Parameters.Add("@IdEmpresa",SqlDbType.BigInt).Value = idEmpresa;

            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<long>> ObtenerPendientesAsync(int maxIntentos)
        {
            var lista = new List<long>();

            using var cn = new SqlConnection(ConnectionString);
            using var cmd = new SqlCommand("SELECT IdEmpresa FROM dbo.ColaSincronizacion_GT " +
                "WHERE Procesado = 0 AND Intentos < @MaxIntentos " +
                "ORDER BY FechaEncolado", cn)
            {
                CommandTimeout = 60
            };
            cmd.Parameters.Add("@MaxIntentos",SqlDbType.Int).Value = maxIntentos;

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while(await dr.ReadAsync())
            {
                lista.Add(Convert.ToInt64(dr["IdEmpresa"]));
            }
            return lista;

        }





    }
}
