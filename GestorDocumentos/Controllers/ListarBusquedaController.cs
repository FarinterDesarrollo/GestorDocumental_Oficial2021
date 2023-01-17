using GestorDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class ListarBusquedaController : Controller
    {
       
        // GET: ListarBusqueda
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        public ActionResult Index(string Nombre, Documento_Detalle model)
        {
            int area = model.AreaId;
            int carpeta = model.CarpetaEncabezadoid;
            List<Documento_Detalle> lst;
            //lst = (from s in db.Documentos_Detalle
            //       where s.llaveUnica.Contains(Nombre) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta
            //       select s).ToList();

            //return View(lst);

            //Agregado: 18/05/2020
            Archivos.DropboxListSubniveles rolenames = new Archivos.DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            // ****************************** Validación de punto en valor de búsqueda
            string NombreconPunto = "";
            NombreconPunto = Nombre.Replace(".", "");
            NombreconPunto = NombreconPunto.ToLower();
            //******************************************************************************************
            if (rname.Contains("JEFE DE FARMACIA"))
            {
                Nombre = Nombre.ToLower();
                lst = (from s in db.Documentos_Detalle
                       where (s.llaveUnica.ToLower().Contains(Nombre) || s.llaveUnica.ToLower().Contains(NombreconPunto)) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta && s.RoleName == rname
                       select s).ToList();
            }
            else
            {
                Nombre = Nombre.ToLower();
                lst = (from s in db.Documentos_Detalle
                       where (s.llaveUnica.ToLower().Contains(Nombre) || s.llaveUnica.ToLower().Contains(NombreconPunto)) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta 
                       select s).ToList();
            }

            ViewBag.BusqCant = "Coincidencias Encontradas:" + lst.Count;
            return View(lst);
        }

        //[HttpPost]
        //public ActionResult Consultar()
        //{
             
           

        //}

        public ActionResult Imagen(int id)
        {
            var context = new Models.ApplicationDbContext();
            string nombre = context.Documentos_Detalle.FirstOrDefault(i => i.Id == id)?.Nombre_Des;
            int posicionpunto = nombre.IndexOf(".");
            string extension = (nombre.Substring(posicionpunto)).ToUpper();
            byte[] imagedata = context.Documentos_Detalle.FirstOrDefault(i => i.Id == id)?.Imagen;
            if (imagedata != null)
            {
                if (extension == ".PNG")
                {
                    return File(imagedata, "image/png");
                }
                if (extension == ".JPG" || extension == ".JPEG")
                {
                    return File(imagedata, "image/jpg");
                }
                if (extension == ".PDF")
                {
                    return File(imagedata, "application/pdf");
                }
                if (extension == ".XLSX" || extension == ".XLS" || extension == ".CSV")
                {
                    return File(imagedata, "application/octet-stream", nombre);
                }
                if (extension == ".DOCX" || extension == ".DOC")
                {
                    return File(imagedata, "application/octet-stream", nombre);
                }

                //return File(imagedata, "image/png");
            }
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
   
}