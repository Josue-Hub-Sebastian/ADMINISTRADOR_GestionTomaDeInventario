using System.Data;
using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Repository.ProcedimientosAlmacenados;
using Microsoft.Data.SqlClient;

namespace Gestion_TomaInventario.Repository.EmpresaRepo
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly IConfiguration _configuration;

         public EmpresaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IReadOnlyList<EmpresaSincronizadaViewModel>> SincronizarEmpresasAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("No se encontro la cadena de conexion 'DefaultConnection'.");
            }

            var empresas = new List<EmpresaSincronizadaViewModel>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            { // aqui mismo ves que se llama a la variable que contiene la cadena de conexion a la bd y tambien la variable del stored procedure que se encuentra en la clase StoredProcedures.cs
                using (SqlCommand cmd = new SqlCommand(StoredProcedures.SincronizarEmpresas, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 120;

                    await cn.OpenAsync();

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        do
                        {
                            if (!TieneColumnasResultadoFinal(reader))
                            {
                                continue;
                            }

                            while (await reader.ReadAsync())
                            {
                                empresas.Add(new EmpresaSincronizadaViewModel
                                {
                                    IdEmpresa = reader.GetInt64(reader.GetOrdinal("IdEmpresa")),
                                    CodEmpresa = LeerString(reader, "CodEmpresa"),
                                    Ruc = LeerString(reader, "Ruc"),
                                    Nombre = LeerString(reader, "Nombre"),
                                    NombreBD = LeerString(reader, "NombreBD"),
                                    Estado = reader.GetBoolean(reader.GetOrdinal("Estado")),
                                });
                            }
                        }
                        while (await reader.NextResultAsync());
                    }
                }
            }

            return empresas;
        }

        private bool TieneColumnasResultadoFinal(SqlDataReader reader)
        {
            // Verificar que tenga las columnas que esperamos
            try
            {
                reader.GetOrdinal("IdEmpresa");
                reader.GetOrdinal("CodEmpresa");
                reader.GetOrdinal("Ruc");
                reader.GetOrdinal("Nombre");
                reader.GetOrdinal("NombreBD");
                return true;  // Si encuentra todas, es el resultset correcto
            }
            catch
            {
                return false;  // Si falta alguna, es otro resultset (como los intermedios)
            }
        }

        private static string? LeerString(SqlDataReader reader, string columna)
        {
            var ordinal = reader.GetOrdinal(columna);
            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        public async Task<IReadOnlyList<LicenciaViewModel>> ListarLicenciasAsync()
        {
            List<LicenciaViewModel> Lista_result = new List<LicenciaViewModel>();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
                }

                using (conexion = new SqlConnection(connectionString))
                {
                    using (comando = new SqlCommand(StoredProcedures.ListarLicencias, conexion))
                    {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = 120;
                        comando.Parameters.Clear();

                        System.Diagnostics.Debug.WriteLine($"?? Ejecutando SP: {StoredProcedures.ListarLicencias}");
                        await conexion.OpenAsync();


                        using (reader = await comando.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                LicenciaViewModel entity = new LicenciaViewModel();

                                entity.IdEmpresa = reader["IdEmpresa"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdEmpresa"]);
                                entity.Nombre = reader["Nombre"] == DBNull.Value ? string.Empty : reader["Nombre"].ToString();
                                entity.Ruc = reader["Ruc"] == DBNull.Value ? string.Empty : reader["Ruc"].ToString();
                                entity.NombreBD = reader["NombreBD"] == DBNull.Value ? string.Empty : reader["NombreBD"].ToString();

                                entity.InicioSuscripcion = reader["InicioSuscripcion"] == DBNull.Value ? null : Convert.ToDateTime(reader["InicioSuscripcion"]);
                                entity.MesesContratados = reader["MesesContratados"] == DBNull.Value ? null : Convert.ToInt32(reader["MesesContratados"]);
                                entity.FinSuscripcion = reader["FinSuscripcion"] == DBNull.Value ? null : Convert.ToDateTime(reader["FinSuscripcion"]);

                                entity.AlmacenesMax = reader["AlmacenesMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AlmacenesMax"]);
                                entity.UbicacionesMax = reader["UbicacionesMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UbicacionesMax"]);
                                entity.ProductosMax = reader["ProductosMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ProductosMax"]);
                                entity.UsuariosAdminMax = reader["UsuariosAdminMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UsuariosAdminMax"]);
                                entity.UsuariosOperadorMax = reader["UsuariosOperadorMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UsuariosOperadorMax"]);
                                entity.InventariosPreparadosMax = reader["InventariosPreparadosMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InventariosPreparadosMax"]);

                                entity.Estado = reader["Estado"] == DBNull.Value ? false : Convert.ToBoolean(reader["Estado"]);

                                Lista_result.Add(entity);
                            }
                        }
                        System.Diagnostics.Debug.WriteLine($"? SP completado. Total registros: {Lista_result.Count}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error en ListarLicenciasAsync: {ex.Message}");
                // falta crear Response para manejar el error, pero por ahora solo lanzamos la excepción
                //response.MENSAJE_ERROR = e.Message.ToString();
                // response.HUBO_ERROR = true;
                throw; // Lanzamos la excepción para mantener el comportamiento original
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }

            return Lista_result;
        }

        public async Task<LicenciaViewModel> ObtenerLicenciaAsync(int id)
        {
            List<LicenciaViewModel> Lista_result = new List<LicenciaViewModel>();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
                }
                using (conexion = new SqlConnection(connectionString))
                using (comando = new SqlCommand(StoredProcedures.ObtenerLicencia, conexion))
                {
                        comando.CommandType = CommandType.StoredProcedure;
                        comando.CommandTimeout = 300;
                        //System.Diagnostics.Debug.WriteLine($"?? Ejecutando SP: {StoredProcedures.ListarLicencias}");
                        //await conexion.OpenAsync();
                  
                    comando.Parameters.Clear();
                    comando.Parameters.Add(new SqlParameter("@IdEmpresa", SqlDbType.BigInt) { Value = id });

                    await conexion.OpenAsync();
                    reader = await comando.ExecuteReaderAsync();
                    if (await reader.ReadAsync())
                    {
                        var entity = new LicenciaViewModel
                        {
                            IdEmpresa = reader["IdEmpresa"] == DBNull.Value ? 0 : Convert.ToInt64(reader["IdEmpresa"]),
                            Nombre = reader["Nombre"] == DBNull.Value ? string.Empty : reader["Nombre"].ToString(),
                            Ruc = reader["Ruc"] == DBNull.Value ? string.Empty : reader["Ruc"].ToString(),
                            NombreBD = reader["NombreBD"] == DBNull.Value ? string.Empty : reader["NombreBD"].ToString(),

                            InicioSuscripcion = reader["InicioSuscripcion"] == DBNull.Value ? null : Convert.ToDateTime(reader["InicioSuscripcion"]),
                            MesesContratados = reader["MesesContratados"] == DBNull.Value ? null : Convert.ToInt32(reader["MesesContratados"]),
                            FinSuscripcion = reader["FinSuscripcion"] == DBNull.Value ? null : Convert.ToDateTime(reader["FinSuscripcion"]),


                            AlmacenesMax = reader["AlmacenesMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AlmacenesMax"]),
                            UbicacionesMax = reader["UbicacionesMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UbicacionesMax"]),
                            ProductosMax = reader["ProductosMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ProductosMax"]),
                            UsuariosAdminMax = reader["UsuariosAdminMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UsuariosAdminMax"]),
                            UsuariosOperadorMax = reader["UsuariosOperadorMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UsuariosOperadorMax"]),
                            InventariosPreparadosMax = reader["InventariosPreparadosMax"] == DBNull.Value ? 0 : Convert.ToInt32(reader["InventariosPreparadosMax"]),
                            Estado = reader["Estado"] == DBNull.Value ? false : Convert.ToBoolean(reader["Estado"]),

                            //emote aqui se añadio mas caposs
                            IdPlan = reader["idPlan"] == DBNull.Value ? null : Convert.ToInt32(reader["idPlan"]),
                            EsPersonalizado = reader["EsPersonalizado"] == DBNull.Value ? false : Convert.ToBoolean(reader["EsPersonalizado"]),
                        };
                        return entity;
                    }
                    return null;
                }
                //emote
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error en ObtenerLicenciaAsync: {ex.Message}");
                throw;
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }
        }





        public async Task<bool> ActualizarLicenciaAsync(LicenciaViewModel licencia)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();

                try
                {
                    using (var cmd = new SqlCommand(StoredProcedures.ActualizarLicencia, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;

                        cmd.Parameters.AddWithValue("@IdEmpresa", licencia.IdEmpresa);
                        cmd.Parameters.AddWithValue("@AlmacenesMax", licencia.AlmacenesMax);
                        cmd.Parameters.AddWithValue("@UbicacionesMax", licencia.UbicacionesMax);
                        cmd.Parameters.AddWithValue("@ProductosMax", licencia.ProductosMax);
                        cmd.Parameters.AddWithValue("@UsuariosAdminMax", licencia.UsuariosAdminMax);
                        cmd.Parameters.AddWithValue("@UsuariosOperadorMax", licencia.UsuariosOperadorMax);
                        cmd.Parameters.AddWithValue("@InventariosPreparadosMax", licencia.InventariosPreparadosMax);
                        cmd.Parameters.AddWithValue("@Estado", licencia.Estado);

                        cmd.Parameters.Add(new SqlParameter("@InicioSuscripcion", SqlDbType.Date)
                        {
                            Value = licencia.InicioSuscripcion.HasValue
                                ? licencia.InicioSuscripcion.Value.Date
                                : DBNull.Value
                        });

                        cmd.Parameters.Add(new SqlParameter("@MesesContratados", SqlDbType.Int)
                        {
                            Value = licencia.MesesContratados.HasValue
                                ? licencia.MesesContratados.Value
                                : DBNull.Value
                        });

 
                        cmd.Parameters.AddWithValue("@IdPlan", (object?)licencia.IdPlan ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EsPER", licencia.EsPersonalizado);


                        var result = await cmd.ExecuteScalarAsync();

                        System.Diagnostics.Debug.WriteLine($"[DB] Resultado scalar SP_ActualizarLicencia: {result}");

                        if (result == null)
                            return false;

                        return Convert.ToInt32(result) > 0;
                    }
                }
                catch (SqlException sqlEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[DB] SqlException en ActualizarLicenciaAsync: {sqlEx}");
                    throw;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[DB] Excepción en ActualizarLicenciaAsync: {ex}");
                    throw;
                }
            }
        }



        public async Task<DashboardEmpresaViewModel?> ObtenerDashboardEmpresaAsync(int idEmpresa)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (cn = new SqlConnection(connectionString))
                {
                    using (cmd = new SqlCommand(StoredProcedures.ObtenerDashboardEmpresa, cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdEmpresa", idEmpresa);
                        await cn.OpenAsync();

                        using (reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync()) 
                            {
                                return new DashboardEmpresaViewModel
                                {
                                    IdEmpresa = Convert.ToInt64(reader["IdEmpresa"]),
                                    NombreEmpresa = reader["NombreEmpresa"].ToString(),
                                    Ruc = reader["RUC"].ToString(),
                                    NombreBD = reader["NombreBD"].ToString(),

                                    EstadoLicencia = Convert.ToBoolean(reader["EstadoLicencia"]),
                                    FechaCreacionBD = Convert.ToDateTime(reader["FechaCreacionBD"]),

                                    ProductosActual = Convert.ToInt32(reader["ProductosActual"]),
                                    ProductosMax = Convert.ToInt32(reader["ProductosMax"]),

                                    AlmacenesActual = Convert.ToInt32(reader["AlmacenesActual"]),
                                    AlmacenesMax = Convert.ToInt32(reader["AlmacenesMax"]),

                                    UbicacionesActual = Convert.ToInt32(reader["UbicacionesActual"]),
                                    UbicacionesMax = Convert.ToInt32(reader["UbicacionesMax"]),

                                    UsuariosAdminActual = Convert.ToInt32(reader["UsuariosAdminActual"]),
                                    UsuariosAdminMax = Convert.ToInt32(reader["UsuariosAdminMax"]),

                                    UsuariosOperadorActual = Convert.ToInt32(reader["UsuariosOperadorActual"]),
                                    UsuariosOperadorMax = Convert.ToInt32(reader["UsuariosOperadorMax"]),

                                    InventariosPreparadosActual = Convert.ToInt32(reader["InventariosActual"]),
                                    InventariosPreparadosMax = Convert.ToInt32(reader["InventariosPreparadosMax"])
                                };
                            }
                        }
                    }
                }
                return null; // Corregido: devolver null si no hay datos
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error en ObtenerDashboardEmpresaAsync: {ex.Message}");
                throw;
            }
            finally
            {
                if (cn != null) { cn.Close(); cn.Dispose(); }
                if (cmd != null) cmd.Dispose();
                if (reader != null) reader.Dispose();
            }
        }


        //canelita
        public async Task<IReadOnlyList<PlanDropdownViewModel>> ListarPlanesActivosAsync()
        {
            List<PlanDropdownViewModel> lista = new List<PlanDropdownViewModel>();
            SqlConnection conexion = null;
            SqlCommand comando = null;
            SqlDataReader reader = null;

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");

                using (conexion = new SqlConnection(connectionString))
                using (comando = new SqlCommand(StoredProcedures.ListarPlanes, conexion))
                {
                    comando.CommandType = CommandType.StoredProcedure;
                    comando.CommandTimeout = 120;
                    comando.Parameters.Clear();
                    comando.Parameters.Add(new SqlParameter("@SoloActivos", SqlDbType.Bit) { Value = true });

                    await conexion.OpenAsync();
                    reader = await comando.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        lista.Add(new PlanDropdownViewModel
                        {
                            IdPlan = reader["id_Plan"] == DBNull.Value ? 0 : Convert.ToInt32(reader["id_Plan"]),
                            NombrePlan = reader["NombrePlan"] == DBNull.Value ? string.Empty : reader["NombrePlan"].ToString(),
                            Almacenes = reader["Almacenes"] == DBNull.Value ? null : Convert.ToInt32(reader["Almacenes"]),
                            Ubicaciones = reader["Ubicaciones"] == DBNull.Value ? null : Convert.ToInt32(reader["Ubicaciones"]),
                            ProductosSkus = reader["Productos_SKUs"] == DBNull.Value ? null : Convert.ToInt32(reader["Productos_SKUs"]),
                            UsuariosAnd = reader["Usuarios_AND"] == DBNull.Value ? null : Convert.ToInt32(reader["Usuarios_AND"]),
                            UsuariosWeb = reader["Usuarios_WEB"] == DBNull.Value ? null : Convert.ToInt32(reader["Usuarios_WEB"]),
                            InventariosPreparadosMax = reader["InventariosPreparadosMax"] == DBNull.Value ? null : Convert.ToInt32(reader["InventariosPreparadosMax"]),
                            PrecioFijo = reader["PrecioFijo"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PrecioFijo"]),
                            PrecioLanzamiento = reader["PrecioLanzamiento"] == DBNull.Value ? null : Convert.ToDecimal(reader["PrecioLanzamiento"]),
                            Estado = reader["Estado"] == DBNull.Value ? false : Convert.ToBoolean(reader["Estado"]) // se añadio estado para que solo traiga los activos
                        });
                    }
                }
            }
            finally
            {
                if (conexion != null) { conexion.Close(); conexion.Dispose(); }
                if (comando != null) comando.Dispose();
                if (reader != null) reader.Dispose();
            }

            return lista;
        }
    }
}

