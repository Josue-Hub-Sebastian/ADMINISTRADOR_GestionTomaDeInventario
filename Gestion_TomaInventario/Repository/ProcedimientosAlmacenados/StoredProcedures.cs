namespace Gestion_TomaInventario.Repository.ProcedimientosAlmacenados
{
    public static class StoredProcedures
    {
        //Se define el nombre del procedimiento almacenado ya despues en los Repositorios se hace la llamada a este procedimiento almacenado como en el android studio
        // login
        public const string ValidarLoginAdmin = "SP_ValidarLoginAdmin";

        #region Empresa
        public const string SincronizarEmpresas = "SP_SincronizarEmpresas";
        public const string ObtenerDashboardEmpresa = "SP_ObtenerDashboardEmpresa";
        #endregion

        #region Licencias
        public const string ListarLicencias = "SP_ListarLicencias";
        public const string ObtenerLicencia = "SP_ObtenerLicencia";
        public const string ActualizarLicencia = "SP_ActualizarLicencia";
        public const string ActualizarLicenciaCliente = "SP_ActualizarLicenciaCliente";
        #endregion

        #region Planes
        public const string ListarPlanes = "SP_ListarPlanes";
        public const string ObtenerPlan = "SP_ObtenerPlan";
        public const string CrearPlan = "SP_CrearPlan";
        public const string ActualizarPlan = "SP_ActualizarPlan";
        public const string CambiarEstadoPlan = "SP_CambiarEstadoPlan";
        #endregion

        #region Backups
        public const string BackupConfigObtener = "SP_BACKUP_CONFIG_OBTENER";
        public const string BackupConfigGuardar = "SP_BACKUP_CONFIG_GUARDAR";
        public const string BackupHistorial = "SP_BACKUP_HISTORIAL_LISTAR";
        public const string BackupHistorialRegistrar = "SP_BACKUP_HISTORIAL_REGISTRAR";
        public const string InstanciaClienteObtener = "SP_INSTANCIA_CLIENTE_OBTENER";
        public const string BackupHistorialMarcadoEliminado = "SP_BACKUP_HISTORIAL_MARCAR_ELIMINADO";
        public const string ListarEmpresasBackupResumen = "SP_EMPRESA_BACKUP_RESUMEN_LISTAR";
        public const string ListarBackupsPendientes = "SP_BACKUP_CONFIG_LISTAR_PENDIENTES";
        #endregion


        #region Contacto_Empresa
        public const string ObtenerContactoEmpresa = "SP_CONTACTO_OBTENER";
        public const string GuardarContactoEmpresa = "SP_CONTACTO_GUARDAR";
        #endregion

    }
}
