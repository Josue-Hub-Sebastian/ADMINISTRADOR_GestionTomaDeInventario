using Gestion_TomaInventario.Services.ColaSincronizacionServ;
using Microsoft.Data.SqlClient;
using System.Data;



namespace Gestion_TomaInventario.Services
{
    public class SyncHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(60);


        public SyncHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<IColaSincronizacionService>();

                var pendientes = await syncService.ObtenerPendientesAsync();

                foreach(var idEmpresa in pendientes)
                {
                    try
                    {
                        var resultado = await syncService.SincronizarEmpresaAsync(idEmpresa);
                        Console.WriteLine($"[{DateTime.Now}] Empresa {idEmpresa}: {(resultado.Success ? "Ok" : "Error")} - {resultado.Message}");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"[{DateTime.Now}] EXCEPTION empresa {idEmpresa}: {ex.Message}");
                    }
                }
                await Task.Delay(_intervalo, stoppingToken);
            }
        }

    }
}
