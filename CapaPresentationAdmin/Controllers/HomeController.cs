using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaEntidad;
using CapaNegocio;

namespace CapaPresentationAdmin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Clientes()
        {
            return View();
        }

        public ActionResult IndexUser()
        {
            return View();
        }

        public ActionResult ClientesUser()
        {
            return View();
        }

        public ActionResult Analisis()
        {
            return View();
        }

        public ActionResult AnalisisUser()
        {
            return View();
        }


        public ActionResult Recomendations()
        {
            return View();
        }

        public ActionResult RecomendationsUser()
        {
            return View();
        }


        [HttpGet]
        public JsonResult ListarRecomendacionesClientes()
        {
            List<Rdcn_Cliente> oLista = new List<Rdcn_Cliente>();
            oLista = new CN_Recomendaciones().Listar_Recomendaciones_Cliente();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ListarRecomendacionesCampanas()
        {
            List<Rdcn_campanas> oLista = new List<Rdcn_campanas>();
            oLista = new CN_Recomendaciones().Listar_Recomendaciones_Campana();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListarRecomendacionesAdicionales()
        {
            List<Rdcn_adicionales> oLista = new List<Rdcn_adicionales>();
            oLista = new CN_Recomendaciones().Listar_Recomendaciones_Adicionales();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ListarRecomendacionesUsuarioCliente()
        {
            List<Rdcn_usuario_cliente> oLista = new List<Rdcn_usuario_cliente>();
            oLista = new CN_Recomendaciones().Listar_Recomendaciones_Usuario_Cliente();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult ListarRecomendacionesUsuarioProducto()
        {
            List<Rdcn_usuario_producto> oLista = new List<Rdcn_usuario_producto>();
            oLista = new CN_Recomendaciones().Listar_Recomendaciones_Usuario_Productos();
            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase fileVentas, HttpPostedFileBase fileProductos, HttpPostedFileBase fileClientes)
        {
            try
            {
                string[] allowedExtensions = { ".xlsx"};
                string path = Server.MapPath("~/API/files/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (fileVentas != null)
                {
                    string extension = Path.GetExtension(fileVentas.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Message"] = "El archivo de ventas debe ser un archivo Excel (.xlsx)";
                        return RedirectToAction("Analisis");
                    }
                    string ventasPath = Path.Combine(path, fileVentas.FileName);
                    fileVentas.SaveAs(ventasPath);
                }
                if (fileProductos != null)
                {
                    string extension = Path.GetExtension(fileProductos.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Message"] = "El archivo de productos debe ser un archivo Excel (.xlsx)";
                        return RedirectToAction("Analisis");
                    }
                    string productosPath = Path.Combine(path, fileProductos.FileName);
                    fileProductos.SaveAs(productosPath);
                }
                if (fileClientes != null)
                {
                    string extension = Path.GetExtension(fileClientes.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Message"] = "El archivo de clientes debe ser un archivo Excel (.xlsx)";
                        return RedirectToAction("Analisis");
                    }
                    string clientesPath = Path.Combine(path, fileClientes.FileName);
                    fileClientes.SaveAs(clientesPath);
                }

                TempData["Message"] = "Archivos subidos exitosamente.";
                return RedirectToAction("Analisis");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error al subir archivos: " + ex.Message;
                return RedirectToAction("Analisis");
            }
        }

        [HttpPost]
        public JsonResult ExecuteScript()
        {
            try
            {
                string batFilePath = Server.MapPath("~/API/main.bat");
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = batFilePath,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    string filesPath = Server.MapPath("~/API/files/");
                    foreach (var file in Directory.GetFiles(filesPath))
                    {
                        System.IO.File.Delete(file);
                    }

                    return Json(new { success = true, message = "Análisis ejecutado correctamente." });
                }
                else
                {
                    string error = process.StandardError.ReadToEnd();
                    return Json(new { success = false, message = "Error al ejecutar el análisis: " + error });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al ejecutar el análisis: " + ex.Message });
            }
        }




    }
}