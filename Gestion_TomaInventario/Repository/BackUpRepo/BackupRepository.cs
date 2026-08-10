using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Models.ViewModels.Backup;
using Gestion_TomaInventario.Repository.ProcedimientosAlmacenados;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace Gestion_TomaInventario.Repository.BackUpRepo
{
    public class BackupRepository : IBackupRepository
    {
        public readonly IConfiguration _configuration;

        public BackupRepository(IConfiguration configuration)
        {
                _configuration = configuration;
        }

        public async Task<bool> GuardarConfiguracionBackupAsync(BackupConfigViewModel model)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada");

                    using SqlConnection cn = new SqlConnection(connectionString);
                    using SqlCommand cmd = new SqlCommand(StoredProcedures.BackupConfigGuardar, cn)
                    {
                        CommandType =System.Data.CommandType.StoredProcedure
                    };

                    //cmd.Parameters.AddWithValue("@IdBackupConfig", model.IdBackupConfig);
                    cmd.Parameters.AddWithValue("@IdEmpresa", model.IdEmpresa);
                    cmd.Parameters.AddWithValue("@RutaDestino", model.RutaDestino);
                    cmd.Parameters.AddWithValue("@Frecuencia", model.Frecuencia);
                    cmd.Parameters.AddWithValue("@DiaSemana", (object)model.DiaSemana ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@DiaMes", (object)model.DiaMes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Hora", model.Hora);
                    cmd.Parameters.AddWithValue("@MaxRespaldos", model.MaxRespaldos);
                    cmd.Parameters.AddWithValue("@Comprimir", model.Comprimir);
                    cmd.Parameters.AddWithValue("@Activo", model.Activo);

                    await cn.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return result != null && Convert.ToInt32(result) == 1;


        }


        public async Task<BackupConfigViewModel> ObtenerConfiguracionBackupAsync(long idEmpresa)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            BackupConfigViewModel model = null;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada");
            }

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open(); 
                using (SqlCommand cmd = new SqlCommand(StoredProcedures.BackupConfigObtener, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmpresa", idEmpresa);
                    
                    
                    using(SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            model = new BackupConfigViewModel()
                            {
                                IdBackupConfig = Convert.ToInt32(dr["IdBackupConfig"]),
                                IdEmpresa = Convert.ToInt64(dr["IdEmpresa"]),
                                NombreEmpresa = dr["NombreEmpresa"].ToString(),
                                RutaDestino = dr["RutaDestino"].ToString(),
                                Frecuencia = dr["Frecuencia"].ToString(),
                                DiaSemana = dr["DiaSemana"] == DBNull.Value ? null : Convert.ToByte(dr["DiaSemana"]),
                                DiaMes = dr["DiaMes"] == DBNull.Value ? null : Convert.ToByte(dr["DiaMes"]),
                                Hora = (TimeSpan)dr["Hora"],
                                MaxRespaldos = Convert.ToInt32(dr["MaxRespaldos"]),
                                Comprimir = Convert.ToBoolean(dr["Comprimir"]),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            };
                        }
                    }
                }
            }
            return model;
        }

        // antes de list era un readonly
        public async Task<List<BackupHistorialViewModel>> ListarHistorialBackupsAsync(long idEmpresa)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Connection string 'Default Connection' no encontrado :v");
        
            var lista = new List<BackupHistorialViewModel>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();
                using (SqlCommand cmd = new SqlCommand(StoredProcedures.BackupHistorial, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmpresa", idEmpresa);

                    using(SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            lista.Add(new BackupHistorialViewModel()
                            {
                                IdHistorial = Convert.ToInt32(dr["IdHistorial"]),
                                IdEmpresa = Convert.ToInt64(dr["IdEmpresa"]),
                                FechaInicio = Convert.ToDateTime(dr["FechaInicio"]),
                                FechaFin = dr["FechaFin"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaFin"]),
                                ArchivoGenerado = dr["ArchivoGenerado"].ToString(),
                                PesoMB = dr["PesoMB"] == DBNull.Value ? null : Convert.ToDecimal(dr["PesoMB"]),
                                Estado = dr["Estado"].ToString(),
                                MensajeError = dr["MensajeError"] == DBNull.Value ? null : dr["MensajeError"].ToString()

                            });
                        }
                    }
                }
                return lista;
            }
        }

        public async Task<int> RegistrarHistorialAsync(BackupHistorialViewModel model)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Connection string 'DefaultConnection' no encontrada");

            using(SqlConnection cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();
                using(SqlCommand cmd = new SqlCommand(StoredProcedures.BackupHistorialRegistrar, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdHistorial", model.IdHistorial == 0 ? DBNull.Value : (object)model.IdHistorial);
                    cmd.Parameters.AddWithValue("@IdEmpresa", model.IdEmpresa);
                    cmd.Parameters.AddWithValue("@FechaInicio", model.FechaInicio);
                    cmd.Parameters.AddWithValue("@FechaFin", model.FechaFin ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ArchivoGenerado", string.IsNullOrWhiteSpace(model.ArchivoGenerado) ? DBNull.Value : (object)model.ArchivoGenerado);
                    cmd.Parameters.AddWithValue("@PesoMB", model.PesoMB ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);
                    cmd.Parameters.AddWithValue("@MensajeError", string.IsNullOrWhiteSpace(model.MensajeError) ? DBNull.Value : (object)model.MensajeError);

                    var result = await cmd.ExecuteScalarAsync();

                    return Convert.ToInt32(result);
                }
            }
        }

        public async Task<InstanciaClienteViewModel?> ObtenerInstanciaClienteAsync(long idEmpresa)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))throw new InvalidOperationException("Connection string 'DefaultConnectiones' no encontrada ");

            InstanciaClienteViewModel? model = null;

            using(SqlConnection cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();
                using(SqlCommand cmd = new SqlCommand(StoredProcedures.InstanciaClienteObtener, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdEmpresa", idEmpresa);

                    using(SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if(await dr.ReadAsync())
                        {
                            model = new InstanciaClienteViewModel
                            {
                                IdInstancia = Convert.ToInt32(dr["IdInstancia"]),
                                IdEmpresa = Convert.ToInt64(dr["IdEmpresa"]),
                                NombreBaseDatos = dr["NombreBD"].ToString() ?? string.Empty,
                                NombreServidor = dr["ServidorSql"].ToString() ?? string.Empty,
                                Estado = Convert.ToBoolean(dr["Estado"])
                            };
                        }
                    }
                }
            }
            return model;
        }

        public async Task MarcarHistoriaEliminadoAsync(long idHistorial)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            using SqlConnection cn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(StoredProcedures.BackupHistorialMarcadoEliminado, cn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };
            cmd.Parameters.AddWithValue("@IdHistorial", idHistorial);
            await cn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<List<EmpresaBackupResumenViewModel>> ListarEmpresasParaBackupAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            var lista = new List<EmpresaBackupResumenViewModel>();
            
            using SqlConnection cn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(StoredProcedures.ListarEmpresasBackupResumen,cn)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            await cn.OpenAsync();
            using SqlDataReader dr = await cmd.ExecuteReaderAsync();

            while(await dr.ReadAsync())
            {
                lista.Add(new EmpresaBackupResumenViewModel
                {
                    IdEmpresa = Convert.ToInt64(dr["IdEmpresa"]),
                    Nombre = dr["Nombre"].ToString() ?? string.Empty,
                    Ruc = dr["Ruc"].ToString() ?? string.Empty,
                    EstadoEmpresa = Convert.ToBoolean(dr["EstadoEmpresa"]),
                    IdInstancia = Convert.ToInt64(dr["IdInstancia"]),
                    NombreBD = dr["NombreBD"].ToString() ?? string.Empty,
                    ServidorSql = dr["ServidorSql"].ToString() ?? string.Empty,
                    EstadoInstancia = Convert.ToBoolean(dr["EstadoInstancia"]),
                    UltimoBackupFecha = dr["UltimoBackupFecha"] == DBNull.Value ? null : Convert.ToDateTime(dr["UltimoBackupFecha"]),
                    UltimoBackupEstado = dr["UltimoBackupEstado"] == DBNull.Value ? null : dr["UltimoBackupEstado"].ToString()
                });
            }
            return lista;
        }

        
    }
}
