using Gestion_TomaInventario.Repository.ProcedimientosAlmacenados;
using Gestion_TomaInventario.Services.BackupServ;
using Microsoft.Data.SqlClient;

namespace Gestion_TomaInventario.Services
{
    public class BackupHostedService : BackgroundService
    {
        //private readonly IBackupService _backupService;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        public BackupHostedService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            //_backupService = backupService;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var empresasPendientes = await ObtenerEmpresasPendientesAsync(_configuration);
                foreach(var idEmpresa in empresasPendientes)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                        var resultado = await backupService.RealizarBackupAsync(idEmpresa);
                        Console.WriteLine($"[{DateTime.Now}] Empresa{idEmpresa}: {(resultado.Success ? "Ok" : "Error")} - {resultado.Message}");
                    }
                    catch (Exception ex) 
                    { 
                        Console.WriteLine($"[{DateTime.Now}] EXCEPCION empresa {idEmpresa}:{ex.Message}");
                    }
                }
                var siguienteEjecucion = CalcularProximaEjecucion();
                var espera = siguienteEjecucion - DateTime.Now;
                if (espera < TimeSpan.Zero) espera = TimeSpan.FromMinutes(5);

                await Task.Delay(espera, stoppingToken);
            }
        }

        private static async Task<List<long>>ObtenerEmpresasPendientesAsync(IConfiguration config)
        {
            var lista = new List<long>();
            string connectionString = config.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Falta 'DefaultConnection' en appsetings.json");
            using SqlConnection cn = new SqlConnection(connectionString);
            using SqlCommand cmd = new SqlCommand(StoredProcedures.ListarBackupsPendientes, cn)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
                CommandTimeout = 120
            };

            await cn.OpenAsync();
            using var dr = await cmd.ExecuteReaderAsync();
            while(await dr.ReadAsync())
            {
                lista.Add(Convert.ToInt64(dr["IdEmpresa"]));
            }
            return lista;
        }
        

        private DateTime CalcularProximaEjecucion()
        {
            //logica segun el plan de cada empresa pero son varias revisar logica :v
            return DateTime.Now.AddMinutes(1);
        }


    }
}
