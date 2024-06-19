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



        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase fileVentas, HttpPostedFileBase fileProductos, HttpPostedFileBase fileClientes)
        {
            try
            {
                string path = Server.MapPath("~/API/files/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (fileVentas != null)
                {
                    string ventasPath = Path.Combine(path, fileVentas.FileName);
                    fileVentas.SaveAs(ventasPath);
                }
                if (fileProductos != null)
                {
                    string productosPath = Path.Combine(path, fileProductos.FileName);
                    fileProductos.SaveAs(productosPath);
                }
                if (fileClientes != null)
                {
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