using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Rdcn_usuario_producto
    {
        public string Usuario { get; set; }
        public string Mes {  get; set; }
        public int Cantidad_producto { get; set; }
        public decimal Total_ventas { get; set; }
        public int Cantidad_cliente { get; set; }
        public string Recomendacion_usuario_producto { get; set; }

    }
}
