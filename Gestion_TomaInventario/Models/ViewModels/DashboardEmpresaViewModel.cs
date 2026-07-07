namespace Gestion_TomaInventario.Models.ViewModels
{
    public class DashboardEmpresaViewModel
    {
        public long IdEmpresa { get; set; }
        public string NombreEmpresa { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public string NombreBD { get; set; } = string.Empty;

        // licencia
        public bool EstadoLicencia { get; set; }

        //fecha de creacion de la base de datos
        public DateTime FechaCreacionBD { get; set; }



        /// ALMACENES  //////////////////////////////////////////////////  
        public int AlmacenesActual { get; set; }
        public int AlmacenesMax { get; set; }
        

        
        /// UBICACIONES //////////////////////////////////////////////////
        public int UbicacionesActual { get; set; }
        public int UbicacionesMax { get; set; }
        


        /// PRODUCTOS //////////////////////////////////////////////////
        public int ProductosActual { get; set; }
        public int ProductosMax { get; set; }
        
        
        
        /// USUARIOS_ADMIN //////////////////////////////////////////////////
        public int UsuariosAdminActual { get; set; }
        public int UsuariosAdminMax { get; set; }
        
        
        
        /// USUARIOS_OPE /////////////////////////////////////////////////
        public int UsuariosOperadorActual { get; set; }
        public int UsuariosOperadorMax { get; set; }
        
        
        
        /// INVENTARIOS_PREPARADOS////////////////////////////////////////////////
        public int InventariosPreparadosActual { get; set; }
        public int InventariosPreparadosMax { get; set; }






        // PROPIEDADES CALCULADAS PARA MOSTRAR EL PORCENTAJE DE USO DE CADA RECURSO


        public decimal ProductosPorcentaje => ProductosMax == 0 ? 0 : ProductosActual * 100M / ProductosMax;

        public decimal AlmacenesPorcentaje => AlmacenesMax == 0 ? 0 : AlmacenesActual * 100M / AlmacenesMax;

        public decimal UbicacionesPorcentaje => UbicacionesMax == 0 ? 0 : UbicacionesActual * 100M / UbicacionesMax;

        public decimal UsuariosAdminPorcentaje => UsuariosAdminMax == 0 ? 0 : UsuariosAdminActual * 100M / UsuariosAdminMax;

        public decimal UsuariosOperadorPorcentaje => UsuariosOperadorMax == 0 ? 0 : UsuariosOperadorActual * 100M / UsuariosOperadorMax;

        public decimal InventariosPreparadosPorcentaje => InventariosPreparadosMax == 0 ? 0 : InventariosPreparadosActual * 100M / InventariosPreparadosMax;
    
    
        
    
    }

}
