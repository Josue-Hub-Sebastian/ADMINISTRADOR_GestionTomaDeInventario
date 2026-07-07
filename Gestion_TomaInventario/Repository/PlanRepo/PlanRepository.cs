using Gestion_TomaInventario.Models;
using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Repository.ProcedimientosAlmacenados;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Gestion_TomaInventario.Repository.PlanRepo
{
    public class PlanRepository : IPlanRepository
    {
        private readonly IConfiguration configuration;

        public PlanRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }



        public async Task<IReadOnlyList<PlanViewModel>> ListarPlanesAsync(bool soloActivos)
        {
            List<PlanViewModel> lista = new List<PlanViewModel>();
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;

            try
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                using( cn = new SqlConnection(connectionString))
                {
                    using (cmd = new SqlCommand(StoredProcedures.ListarPlanes, cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@SoloActivos", SqlDbType.Bit) { Value = soloActivos});

                        await cn.OpenAsync();

                        using (dr = await cmd.ExecuteReaderAsync())
                        {
                            while(await dr.ReadAsync())
                            {
                                lista.Add(new PlanViewModel
                                {
                                    IdPlan = dr["id_Plan"] == DBNull.Value ? 0 : Convert.ToInt32(dr["id_Plan"]),
                                    NombrePlan = dr["NombrePlan"] == DBNull.Value ? string.Empty : dr["NombrePlan"].ToString(),

                                    Almacenes = dr["Almacenes"] == DBNull.Value ? null : Convert.ToInt32(dr["Almacenes"]),
                                    Ubicaciones = dr["Ubicaciones"] == DBNull.Value ? null : Convert.ToInt32(dr["Ubicaciones"]),
                                    ProductosSkus = dr["Productos_SKUs"] == DBNull.Value ? null : Convert.ToInt32(dr["Productos_SKUs"]),
                                    UsuariosAnd = dr["Usuarios_AND"] == DBNull.Value ? null : Convert.ToInt32(dr["Usuarios_AND"]),
                                    UsuariosWeb = dr["Usuarios_WEB"] == DBNull.Value ? null : Convert.ToInt32(dr["Usuarios_WEB"]),
                                    InventariosPreparadosMax = dr["InventariosPreparadosMax"] == DBNull.Value ? null : Convert.ToInt32(dr["InventariosPreparadosMax"]),

                                    PrecioFijo = dr["PrecioFijo"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PrecioFijo"]),
                                    PrecioLanzamiento = dr["PrecioLanzamiento"] == DBNull.Value ? null : Convert.ToDecimal(dr["PrecioLanzamiento"]),
                                    Estado = dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error en ListarPlanesAsync: {ex.Message}");
                throw;

            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                    dr.Close();
                if (cmd != null)
                    cmd.Dispose();
                if (cn != null && cn.State == System.Data.ConnectionState.Open)
                    cn.Close();
            }
            return lista;

        }

        public async Task<PlanFormViewModel?> ObtenerPlanAsync(int idPlan)
        {
            SqlConnection cn = null;
            SqlCommand cmd = null;
            SqlDataReader dr = null;

            try
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                using (cn = new SqlConnection(connectionString))
                {
                    await cn.OpenAsync();
                    using (cmd = new SqlCommand(StoredProcedures.ObtenerPlan, cn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.CommandTimeout = 120;
                        cmd.Parameters.Clear();

                        cmd.Parameters.AddWithValue("@idPlan", idPlan);

                        dr = await cmd.ExecuteReaderAsync();

                        if(await dr.ReadAsync())
                        {
                            return new PlanFormViewModel
                            {
                                IdPlan = dr["id_Plan"] == DBNull.Value ? 0 : Convert.ToInt32(dr["id_Plan"]),
                                NombrePlan = dr["NombrePlan"] == DBNull.Value ? string.Empty : dr["NombrePlan"].ToString(),
                                Almacenes = dr["Almacenes"] == DBNull.Value ? null : Convert.ToInt32(dr["Almacenes"]),
                                Ubicaciones = dr["Ubicaciones"] == DBNull.Value ? null : Convert.ToInt32(dr["Ubicaciones"]),
                                ProductosSkus = dr["Productos_SKUs"] == DBNull.Value ? null : Convert.ToInt32(dr["Productos_SKUs"]),
                                UsuariosAnd = dr["Usuarios_AND"] == DBNull.Value ? null : Convert.ToInt32(dr["Usuarios_AND"]),
                                UsuariosWeb = dr["Usuarios_WEB"] == DBNull.Value ? null : Convert.ToInt32(dr["Usuarios_WEB"]),
                                InventariosPreparadosMax = dr["InventariosPreparadosMax"] == DBNull.Value ? null : Convert.ToInt32(dr["InventariosPreparadosMax"]),
                                
                                PrecioFijo = dr["PrecioFijo"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PrecioFijo"]),
                                PrecioLanzamiento = dr["PrecioLanzamiento"] == DBNull.Value ? null : Convert.ToDecimal(dr["PrecioLanzamiento"]),
                                Estado = dr["Estado"] != DBNull.Value && Convert.ToBoolean(dr["Estado"])
                            };
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error en ObtenerPlanAsync: {ex.Message}");
                throw;
            }
            finally
            {
                if (dr != null && !dr.IsClosed)
                    dr.Close();
                if (cmd != null)
                    cmd.Dispose();
                if (cn != null && cn.State == System.Data.ConnectionState.Open)
                    cn.Close();
            }

        }



        public async Task<ResultadoOperacion> CrearPlanAsync(PlanFormViewModel model)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            using (var cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();
                using(var cmd = new SqlCommand(StoredProcedures.CrearPlan, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@NombrePlan", model.NombrePlan);
                    cmd.Parameters.AddWithValue("@Almacenes", model.Almacenes);
                    cmd.Parameters.AddWithValue("@Ubicaciones",model.Ubicaciones);
                    cmd.Parameters.AddWithValue("@Productos_SKUs",model.ProductosSkus);
                    cmd.Parameters.AddWithValue("@Usuarios_AND",model.UsuariosAnd);
                    cmd.Parameters.AddWithValue("@Usuarios_WEB",model.UsuariosWeb);
                    cmd.Parameters.AddWithValue("@InventariosPreparadosMax",model.InventariosPreparadosMax);
                    cmd.Parameters.AddWithValue("@PrecioFijo", model.PrecioFijo);
                    cmd.Parameters.AddWithValue("@PrecioLanzamiento",model.PrecioLanzamiento);
                    cmd.Parameters.AddWithValue("@Estado", model.Estado);

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        return new ResultadoOperacion
                        {
                            Exito = true,
                            Mensaje = "Plan creado correctamente."
                        };

                    }
                    catch(SqlException ex)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = ex.Message
                        };
                    }
                    catch
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = "Ocurrió un error inesperado al crear el plan."
                        };
                    }
                }
            }
        }


        //
        public async Task<ResultadoOperacion> ActualizarPlanAsync(PlanFormViewModel model)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            using (var cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand(StoredProcedures.ActualizarPlan, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@idPlan", model.IdPlan);
                    cmd.Parameters.AddWithValue("@Almacenes", model.Almacenes);
                    cmd.Parameters.AddWithValue("@Ubicaciones", model.Ubicaciones);
                    cmd.Parameters.AddWithValue("@Productos_SKUs", model.ProductosSkus);
                    cmd.Parameters.AddWithValue("@Usuarios_AND", model.UsuariosAnd);
                    cmd.Parameters.AddWithValue("@Usuarios_WEB", model.UsuariosWeb);
                    cmd.Parameters.AddWithValue("@InventariosMax", model.InventariosPreparadosMax);
                    cmd.Parameters.AddWithValue("@PrecioFijo", model.PrecioFijo);
                    cmd.Parameters.AddWithValue("@PrecioLanzamiento", model.PrecioLanzamiento);
                    try
                    {
                        var result = await cmd.ExecuteNonQueryAsync();

                        return new ResultadoOperacion
                        {
                            Exito = true,
                            Mensaje = "Plan actualizado correctamente."
                        };
                    }
                    catch(SqlException ex)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = ex.Message
                        };
                    }
                    catch
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = "Ocurrió un error inesperado al actualizar el plan."
                        };
                    }
                }
            }
        }


        public async Task<ResultadoOperacion> CambiarEstadoPlanAsync(int idPlan, bool estado)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            using (var cn = new SqlConnection(connectionString))
            {
                await cn.OpenAsync();

                using (var cmd = new SqlCommand(StoredProcedures.CambiarEstadoPlan, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@idPlan", idPlan);
                    cmd.Parameters.AddWithValue("@Estado", estado);

                    try
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        int filas = 0;

                        if (result != null && int.TryParse(result.ToString(), out var r))
                            filas = r;

                        if (filas > 0)
                        {
                            return new ResultadoOperacion
                            {
                                Exito = true,
                                Mensaje = estado
                                    ? "Plan activado correctamente."
                                    : "Plan desactivado correctamente."
                            };
                        }

                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = "No se pudo actualizar el estado del plan."
                        };
                    }
                    catch (SqlException ex)
                    {
                        // Captura el RAISERROR del SP
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = ex.Message
                        };
                    }
                    catch (Exception)
                    {
                        return new ResultadoOperacion
                        {
                            Exito = false,
                            Mensaje = "Ocurrió un error inesperado al cambiar el estado del plan."
                        };
                    }
                }
            }
        }




    }
}
