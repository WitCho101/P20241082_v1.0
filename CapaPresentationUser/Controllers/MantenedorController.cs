using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentationUser.Controllers
{
    public class MantenedorController : Controller
    {
        // GET: Mantenedor
        public ActionResult Tickets()
        {
            return View();
        }

        public ActionResult PreguntasFrecuentes()
        {
            return View();
        }


        [HttpGet]
        public JsonResult ListarTickets()
        {
            List<Ticket> oLista = new List<Ticket>();
            oLista = new CN_Ticket().Listar();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarTicket(Ticket objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            objeto.oUsuario.IdUsuario = ((Usuarios)Session["Usuario"]).IdUsuario;

            if (objeto.IdTicket == 0)
            {
                resultado = new CN_Ticket().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Ticket().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EliminarTicket(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;


            respuesta = new CN_Ticket().Eliminar(id, out mensaje);

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }
    }
}