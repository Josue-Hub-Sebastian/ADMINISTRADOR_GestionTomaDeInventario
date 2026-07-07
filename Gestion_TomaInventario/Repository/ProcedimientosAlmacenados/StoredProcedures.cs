namespace Gestion_TomaInventario.Repository.ProcedimientosAlmacenados
{
    public static class StoredProcedures
    {
        //Se define el nombre del procedimiento almacenado ya despues en los Repositorios se hace la llamada a este procedimiento almacenado como en el android studio
        // login
        public const string ValidarLoginAdmin = "SP_ValidarLoginAdmin";
        
        //empresa 
        public const string SincronizarEmpresas = "SP_SincronizarEmpresas";
        public const string ObtenerDashboardEmpresa = "SP_ObtenerDashboardEmpresa";

        //licencias
        public const string ListarLicencias = "SP_ListarLicencias";
        public const string ObtenerLicencia = "SP_ObtenerLicencia";
        public const string ActualizarLicencia = "SP_ActualizarLicencia";
        public const string ActualizarLicenciaCliente = "SP_ActualizarLicenciaCliente";

        //Planes
        public const string ListarPlanes = "SP_ListarPlanes";
        public const string ObtenerPlan = "SP_ObtenerPlan";
        public const string CrearPlan = "SP_CrearPlan";
        public const string ActualizarPlan = "SP_ActualizarPlan";
        public const string CambiarEstadoPlan = "SP_CambiarEstadoPlan";

    }
}
