namespace Gestion_TomaInventario.Services.ColaSincronizacionServ
{
    public interface IColaSincronizacionService
    {
        Task<List<long>> ObtenerPendientesAsync();
        Task<(bool Success, string Message)> SincronizarEmpresaAsync(long idEmpresa);
    }
}
