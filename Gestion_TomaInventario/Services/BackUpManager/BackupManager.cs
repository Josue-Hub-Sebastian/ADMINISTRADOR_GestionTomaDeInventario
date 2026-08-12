using Gestion_TomaInventario.Models.ViewModels;
using Gestion_TomaInventario.Models.ViewModels.Backup;
using Gestion_TomaInventario.Repository.BackUpRepo;
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace Gestion_TomaInventario.Services.BackUpManager
{
    public class BackupManager : IBackupManager
    {
        private readonly IBackupRepository _backupRepository;
        private readonly IConfiguration _configuration;

        public BackupManager(IBackupRepository backupRepository, IConfiguration configuration)
        {
            _backupRepository = backupRepository;
            _configuration = configuration;
        }

        public async Task<BackupResult> EjecutarBackupAsync(long idEmpresa)
        {
            // Obtener configuración
            BackupConfigViewModel? configuracion = await _backupRepository.ObtenerConfiguracionBackupAsync(idEmpresa);
            if (configuracion == null)
                return Error("No existe configuración de backup para la empresa.");

            // Obtener instancia
            InstanciaClienteViewModel? instancia = await _backupRepository.ObtenerInstanciaClienteAsync(idEmpresa);
            if (instancia == null)
                return Error("No existe una instancia asociada a la empresa.");
            if (!instancia.Estado)
                return Error("La instancia del cliente se encuentra deshabilitada.");
            if (!configuracion.Activo)
                return Error("La configuración de backup está deshabilitada.");
            if (!Directory.Exists(configuracion.RutaDestino))
                return Error($"La carpeta '{configuracion.RutaDestino}' no existe.");

            // Preparar archivo
            string nombreArchivo = $"{instancia.NombreBaseDatos}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
            string rutaBackup = Path.Combine(configuracion.RutaDestino, nombreArchivo);
            //contar archvos de la carpeta (por la ruta)

            var historial = new BackupHistorialViewModel
            {
                IdEmpresa = idEmpresa,
                FechaInicio = DateTime.Now,
                ArchivoGenerado = nombreArchivo,
                Estado = "EN_PROCESO"
            };

            int idHistorial = await _backupRepository.RegistrarHistorialAsync(historial);

            try
            {
                // Ejecutar backup
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using SqlConnection cn = new SqlConnection(connectionString);
                await cn.OpenAsync();

                string sql = @"
                    BACKUP DATABASE [{0}]
                    TO DISK = @Ruta
                    WITH INIT";
                sql = string.Format(sql, instancia.NombreBaseDatos);

                using SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@Ruta", rutaBackup);
                await cmd.ExecuteNonQueryAsync();

          

                if (!File.Exists(rutaBackup))
                {
                    throw new Exception("SQL indicó que terminó el BACKUP, pero el archivo .bak no existe.");
                }
                //zip
                string archivoFinal = nombreArchivo;
                string rutaFinal = rutaBackup;

                if (configuracion.Comprimir)
                {
                    try
                    {
                        rutaFinal = await ComprimirBackupAsync(rutaBackup);
                        archivoFinal = Path.GetFileName(rutaFinal);

                    }
                    catch (Exception ex)
                    {
                        historial.MensajeError = $"La compresion fallo {ex.Message}";
                    }
                }
                    // Calcular peso
                FileInfo archivo = new FileInfo(rutaFinal);
                decimal pesoMb = Math.Round(archivo.Length / 1024m / 1024m, 2);

                historial.IdHistorial = idHistorial;
                historial.FechaFin = DateTime.Now;
                historial.Estado =string.IsNullOrWhiteSpace(historial.MensajeError)? "EXITOSO": "ZIP";
                historial.PesoMB = pesoMb;
                historial.ArchivoGenerado = archivoFinal;


                await _backupRepository.RegistrarHistorialAsync(historial);

               //  await _backupRepository.GuardarConfiguracionBackupAsync(model);
                // Limpiar backups antiguos
                await LimpiarBackupsAntiguosAsync(idEmpresa);

                return new BackupResult
                {
                    Success = true,
                    Message = historial.MensajeError == null ? "Backup realizado Correctamente" : "Backup Realizado Correctamente, pero no fue posible comprimir el archivo"
                };
            }
            catch (Exception ex)
            {
                historial.IdHistorial = idHistorial;
                historial.FechaFin = DateTime.Now;
                historial.Estado = "ERROR";
                historial.MensajeError = ex.Message;
                await _backupRepository.RegistrarHistorialAsync(historial);

                return new BackupResult
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // C#
        public async Task LimpiarBackupsAntiguosAsync(long idEmpresa)
        {
            var configuracion = await _backupRepository.ObtenerConfiguracionBackupAsync(idEmpresa);
            if (configuracion == null || configuracion.MaxRespaldos <= 0) return;

            var historial = await _backupRepository.ListarHistorialBackupsAsync(idEmpresa);

            var vigentes = historial
                .Where(h => h.Estado == "EXITOSO" || h.Estado == "ZIP")
                .OrderByDescending(h => h.FechaInicio) 
                .ToList();

            var aEliminar = vigentes.Skip(configuracion.MaxRespaldos).ToList();

            foreach (var viejo in aEliminar)
            {
                if (string.IsNullOrWhiteSpace(viejo.ArchivoGenerado))
                {
                    //await _backupRepository.MarcarHistoriaEliminadoAsync(viejo.IdHistorial);
                    continue;
                }

                var rutaBase = Path.Combine(configuracion.RutaDestino, viejo.ArchivoGenerado);
                // borren todo sin discriminar
                try
                {
                    if (File.Exists(rutaBase))
                    {
                        File.Delete(rutaBase);
                    }
                    else
                    {
                        var rutaBak = Path.ChangeExtension(rutaBase, ".bak");
                        var ruta7z = Path.ChangeExtension(rutaBase, ".7z");
                        if (File.Exists(rutaBak)) File.Delete(rutaBak);
                        if (File.Exists(ruta7z)) File.Delete(ruta7z);
                    }
                }
                catch(IOException ex)
                {
                    Console.WriteLine(ex.ToString());   
                }
                //await _backupRepository.MarcarHistoriaEliminadoAsync(viejo.IdHistorial);
            }
        }

        private async Task<string> ComprimirBackupAsync(string rutaBackup)
        {
            // no pregunten por esto por favor
            string sevenZipPath = _configuration["Backup:SevenZipPath"] ?? @"C:\Program Files\7-Zip\7z.exe";
            string passwordZip = _configuration["Backup:ZipPassword"]
                ?? throw new InvalidOperationException("Falta configurar 'Backup:ZipPassword' en appsettings/secrets.");

            if (!File.Exists(sevenZipPath))
                throw new FileNotFoundException($"No se encontró 7-Zip en '{sevenZipPath}'.");

            string rutaZip = Path.ChangeExtension(rutaBackup, ".7z");

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = sevenZipPath,
                // -mhe=on cifra también los nombres/metadata dentro del .7z, no solo el contenido
                Arguments = $"a -t7z \"{rutaZip}\" \"{rutaBackup}\" -p{passwordZip} -mhe=on -mx=5",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proceso = System.Diagnostics.Process.Start(psi)
                ?? throw new InvalidOperationException("No se pudo iniciar el proceso de 7-Zip.");

            string salidaError = await proceso.StandardError.ReadToEndAsync();
            await proceso.WaitForExitAsync();

            if (proceso.ExitCode != 0)
                throw new InvalidOperationException($"7-Zip terminó con código {proceso.ExitCode}: {salidaError}");

            File.Delete(rutaBackup);
            //File.Delete(rutaZip);
            return rutaZip;
        }

        private BackupResult Error(string mensaje) => new BackupResult
        {
            Success = false,
            Message = mensaje
        };
    }
}
