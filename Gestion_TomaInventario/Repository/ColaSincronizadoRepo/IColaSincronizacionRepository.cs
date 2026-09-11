namespace Gestion_TomaInventario.Repository.ColaSincronizadoRepo
{
    public interface IColaSincronizacionRepository
    {
        Task<List<long>> ObtenerPendientesAsync(int maxIntentos);
        Task MarcarProcesadoAsync(long idEmpresa);
        Task MarcarErrorAsync(long idEmpresa, string error);
    }
}
