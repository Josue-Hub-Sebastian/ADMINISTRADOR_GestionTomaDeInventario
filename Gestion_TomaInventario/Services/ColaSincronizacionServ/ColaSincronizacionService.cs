
using Gestion_TomaInventario.Repository.ColaSincronizadoRepo;
using Gestion_TomaInventario.Repository.ProcedimientosAlmacenados;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Gestion_TomaInventario.Services.ColaSincronizacionServ
{
    public class ColaSincronizacionService : IColaSincronizacionService
    {
        private readonly IColaSincronizacionRepository _repository;
        private readonly IConfiguration _configuration;
        private const int MaxIntentos = 5;  

        public ColaSincronizacionService(IColaSincronizacionRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        //public Task<List<long>> ObtenerPendientesAsync(int maxIntentos) => _repository.ObtenerPendientesAsync(MaxIntentos);

        public async Task<(bool Success, string Message)> SincronizarEmpresaAsync(long idEmpresa)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Falta 'DefaultConnection' en appsetings.json");

                using var cn = new SqlConnection(connectionString);
                using var cmd = new SqlCommand(StoredProcedures.ActualizarLicenciaCliente,cn)
                {
                    CommandType = System.Data.CommandType.StoredProcedure,
                    CommandTimeout = 120
                };

                cmd.Parameters.Add("@IdEmpresa", SqlDbType.BigInt).Value = idEmpresa;

                await cn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                await _repository.MarcarProcesadoAsync(idEmpresa);
                return (true, "Sincronizado correctamente");
            }
            catch(Exception ex)
            {
                await _repository.MarcarErrorAsync(idEmpresa, ex.Message);
                return(false,ex.Message);
            }
        }

        public Task<List<long>> ObtenerPendientesAsync() => _repository.ObtenerPendientesAsync(MaxIntentos); 
        
    }
}
