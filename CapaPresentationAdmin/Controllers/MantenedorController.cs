using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaEntidad;
using CapaNegocio;

namespace CapaPresentationAdmin.Controllers
{
    [Authorize]
    public class MantenedorController : Controller
    {
        // GET: Mantenedor
        public ActionResult Usuarios()
        {
            return View();
        }

        public ActionResult Tickets()
        {
            return View();
        }

        public ActionResult PreguntasFrecuentes()
        {
            return View();
        }

        public ActionResult TicketsUser()
        {
            var usuario = Session["Usuario"] as Usuarios;

            if (usuario != null)
            {
                ViewBag.IdUsuario = usuario.IdUsuario;
                ViewBag.Nombre = usuario.Nombre;
                ViewBag.Correo = usuario.Correo;
            }

            return View();
        }

        public ActionResult PreguntasFrecuentesUser()
        {
            return View();
        }



        [HttpGet]
        public JsonResult ListarUsuarios()
        {
            List<Usuarios> oLista = new List<Usuarios>();
            oLista = new CN_Usuarios().Listar();
            return Json(new { data = oLista}, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarUsuario(Usuarios objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdUsuario == 0){
                resultado = new CN_Usuarios().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Usuarios().Editar(objeto, out mensaje);
            }

            return Json(new { resultado = resultado,mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;


            respuesta = new CN_Usuarios().Eliminar(id, out mensaje);

            return Json(new { respuesta = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
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

            var usuario = Session["Usuario"] as Usuarios;

            objeto.oUsuario = new Usuarios
            {
                IdUsuario = usuario.IdUsuario,
                Nombre = usuario.Nombre,
                Correo = usuario.Correo
            };

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