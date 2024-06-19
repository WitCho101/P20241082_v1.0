using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Recomendaciones
    {
        private CD_Recomendaciones objCapaDato = new CD_Recomendaciones();

        public List<Rdcn_Cliente> Listar_Recomendaciones_Cliente()
        {
            return objCapaDato.Listar_Recomendaciones_Clientes();
        }

        public List<Rdcn_campanas> Listar_Recomendaciones_Campana()
        {
            return objCapaDato.Listar_Recomendaciones_Campanas();
        }

        public List<Rdcn_adicionales> Listar_Recomendaciones_Adicionales()
        {
            return objCapaDato.Listar_Recomendaciones_Adicionales();
        }

        public List<Rdcn_usuario_cliente> Listar_Recomendaciones_Usuario_Cliente()
        {
            return objCapaDato.Listar_Recomendaciones_Usuario_Cliente();
        }

        public List<Rdcn_usuario_producto> Listar_Recomendaciones_Usuario_Productos()
        {
            return objCapaDato.Listar_Recomendaciones_Usuario_Productos();
        }

    }
}
