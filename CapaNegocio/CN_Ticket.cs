using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Ticket
    {
        private CD_Tickets objCapaDato = new CD_Tickets();

        public List<Ticket> Listar()
        {
            return objCapaDato.Listar();
        }

        public int Registrar(Ticket obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Asunto) || string.IsNullOrWhiteSpace(obj.Asunto))
            {
                Mensaje = "El asunto no puede ser vacio";
            }
            else if (obj.oUsuario.IdUsuario == 0)
            {
                Mensaje = "Debe seleccionar un Usuario";
            }


            if (string.IsNullOrEmpty(Mensaje))
            {

                string asunto = "Creación de ticket";
                string mensaje_correo = "<h3>Se generó un tickect</h3></br><p>El asunto es: !asunto!</p></br><p>La prioridad es: !prioridad!</p>";
                mensaje_correo = mensaje_correo.Replace("!asunto!", obj.Asunto);

                if(obj.Prioridad)
                {
                    mensaje_correo = mensaje_correo.Replace("!prioridad!", "Baja");
                }
                else
                {
                    mensaje_correo = mensaje_correo.Replace("!prioridad!", "Alta");
                }


                bool respuesta = CN_Recursos.EnviarCorreo(obj.oUsuario.Correo, asunto, mensaje_correo);

                if (respuesta)
                {
                    return objCapaDato.Registrar(obj, out Mensaje);
                }
                else
                {
                    Mensaje = "No se puede enviar el correo";
                    return 0;
                }


            }
            else
            {
                return 0;
            }
        }




        public bool Editar(Ticket obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (string.IsNullOrEmpty(obj.Asunto) || string.IsNullOrWhiteSpace(obj.Asunto))
            {
                Mensaje = "El asunto no puede ser vacio";
            }
            else if (obj.oUsuario.IdUsuario == 0)
            {
                Mensaje = "Debe seleccionar un Usuario";
            }


            if (string.IsNullOrEmpty(Mensaje))
            {
                return objCapaDato.Editar(obj, out Mensaje);
            }
            else
            {
                return false;
            }
        }




        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);
        }




    }
}
