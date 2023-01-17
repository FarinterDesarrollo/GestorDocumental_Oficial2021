using GestorDocumentos.Archivos;
using GestorDocumentos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestorDocumentos.Clases;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class Reporte_DocumentosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: VisualizacionConFiltros
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public Reporte_DocumentosController()
        {
        }

        public Reporte_DocumentosController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Reporte_Documentos
        public ActionResult Index()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            var usuario = (from u in db.Users
                           where u.UserName == User.Identity.Name
                           select new { u.Id }).ToString();

            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            var areas = (from a in db.Areas
                         join axr in db.RoleXAreas on a.Id equals axr.AreaId
                         //where axr.RoleName == rname
                         select new { a.Id, a.Descripcion }).Distinct().ToList();

            if (areas.Count == 0)
            {
                var areasxcarpeta = (from a in db.Areas
                                     join axrxc in db.RoleXAreaXCarpetas on
                                     a.Id equals axrxc.AreaId
                                     //where axrxc.RoleName == rname
                                     select new { a.Id, a.Descripcion }).Distinct().ToList(); //cambio
                if (areasxcarpeta.Count == 0)
                {
                    var areasxcarpetaxubniveles = (from a in db.Areas
                                                   join axrxcxs in db.RoleXAreaXCarpetasXSubniveles on
                                                   a.Id equals axrxcxs.AreaId
                                                   //where axrxcxs.RoleName == rname
                                                   select new { a.Id, a.Descripcion }).Distinct().ToList();
                    foreach (var item in areasxcarpetaxubniveles)
                    {
                        list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
                    }
                }
                else
                {
                    foreach (var item in areasxcarpeta)
                    {
                        list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
                    }
                }


            }
            else
            {
                foreach (var item in areas)
                {
                    list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
                }
            }

            if (list.Count == 0)
            {
                ViewBag.AreaId = list;
                ViewBag.CarpetaEncabezadoid = list;
            }
            else
            {
                ViewBag.AreaId = list;
                //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion");
                ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion");
            }


            return View();
        }

        public class listafecha
        {
            public int Campo_TipoId { get; set; }
            public string Descripcion { get; set; }
            public int Tipo { get; set; }
        }

        [HttpPost]
        public JsonResult fields(int id)
        {
            listafecha fecha = new listafecha();
            listafecha listafecha = new listafecha();

            //Cambio Agregado el 07/05/2020
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            var Efarmacia = (from ms in db.MantenimientoSubniveles
                             join rxaxcxs in db.RoleXAreaXCarpetasXSubniveles
on ms.CarpetaEncabezadoid equals rxaxcxs.CarpetaEncabezadoid
                             where ms.Id == rxaxcxs.MantenimientoSubnivelesid && rxaxcxs.RoleName == rname
                             && ms.CarpetaEncabezadoid == id
                             select ms.Id).ToList();

            var mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id);

            if (Efarmacia.Count > 0)
            {
                int numero = Efarmacia[0];
                //mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero));
                mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6") && !ms.Descripcion.Contains("H0") && !ms.Descripcion.Contains("H1") && !ms.Descripcion.Contains("H2") && !ms.Descripcion.Contains("H5") && !ms.Descripcion.Contains("H6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero));
            }

            //var mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id);
            // var selects = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id).Select(n => new { n.ConfCarpetaDetalle.Descripcion, n.ConfCarpetaDetalleId }).Distinct().ToList();
            var tt = JsonConvert.SerializeObject(mn.ToList());
            var IsFecha = db.ConfCarpetaDetalle.Where(fs => fs.CarpetaEncabezadoid == id).Select(tc => new { tc.Tipo_Campo.Descripcion, tc.Tipo_campoId }).Where(tc => tc.Descripcion == "Fecha").ToList();
            var IsFechayhora = db.ConfCarpetaDetalle.Where(fs => fs.CarpetaEncabezadoid == id).Select(tc => new { tc.Tipo_Campo.Descripcion, tc.Tipo_campoId }).Where(tc => tc.Descripcion == "Fecha y Hora").ToList();
            var query = (from unionccpms in
                           (from ms in db.MantenimientoSubniveles
                            join ccd in db.ConfCarpetaDetalle on ms.CarpetaEncabezadoid equals ccd.CarpetaEncabezadoid

                            where ms.CarpetaEncabezadoid == id && ms.AreaId == ccd.AreaId && ccd.CarpetaEncabezadoid == id && ms.ConfCarpetaDetalleId == ccd.Id
                            select new { ms.ConfCarpetaDetalle.Descripcion, ms.ConfCarpetaDetalleId, ccd.NivelId, ccd.CarpetaEncabezadoid })
                         where unionccpms.CarpetaEncabezadoid == id
                         orderby unionccpms.NivelId
                         select new { unionccpms.Descripcion, unionccpms.ConfCarpetaDetalleId }
           ).Distinct().ToList();

            if (IsFecha.Count >= 1)
            {

                fecha.Campo_TipoId = IsFecha[0].Tipo_campoId;
                fecha.Descripcion = IsFecha[0].Descripcion;
                fecha.Tipo = 1;

            }
            else if (IsFechayhora.Count >= 1)
            {

                fecha.Campo_TipoId = IsFechayhora[0].Tipo_campoId;
                fecha.Descripcion = IsFechayhora[0].Descripcion;
                fecha.Tipo = 2;

            }
            //return Json(new { options = JsonConvert.SerializeObject(mn.ToList()), selects = JsonConvert.SerializeObject(selects), fecha = JsonConvert.SerializeObject(fecha) });
            return Json(new { options = JsonConvert.SerializeObject(mn.ToList()), selects = JsonConvert.SerializeObject(query), fecha = JsonConvert.SerializeObject(fecha) });
        }

        public JsonResult GetAreayEncabezadoCarpetaId(int idarea)
        {
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            //var listaareas = subniveles.GetAreayEncabezadoCarpetaIdpermisos(idarea, rname);
            var listaareas = subniveles.GetAreayEncabezadoCarpetaId(idarea );
            return Json(listaareas, JsonRequestBehavior.AllowGet);
        }

        public class datos_reporte 
            {
            public string Id { get; set; }
            public string Descripcion_area { get; set; } 
            public string carpeta_Descripcion { get; set; }
            public string llaveUnica { get; set; }
            public string Nombre_Ori { get; set; }
            public string Nombre_Des { get; set; }
            public DateTime FechaRegistro { get; set; }
            public DateTime FechaActualizacion { get; set; }
            public DateTime FechaUsuario { get; set; } //LUEGO CAMBIAR A DATETIME
        }

        public string llavedescodificada(int carpetaid, int areaid, string llaveunica)
        {
            string llaveunicadecodificada = "";
            llaveunicadecodificada = llaveunica;
            var MantenimientoSubNiveles = db.MantenimientoSubniveles.Where(c => c.CarpetaEncabezadoid == carpetaid && c.AreaId == areaid).ToList();
            foreach (var item in MantenimientoSubNiveles)
            {
                llaveunicadecodificada = llaveunicadecodificada.Replace(item.Descripcion, "");
            }

            llaveunicadecodificada = llaveunicadecodificada.Replace("-", "");

            return llaveunicadecodificada.ToString();
        }

        public ActionResult GenerarReporte(string Nombre, Documento_Detalle model)
        {
            int area = model.AreaId;
            int carpeta = model.CarpetaEncabezadoid;
            List<datos_reporte> lst;
            int existefecha = 0;
            Archivos.DropboxListSubniveles rolenames = new Archivos.DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            //var MantenimientoSubniveles = db.MantenimientoSubniveles.Where(c => c.CarpetaEncabezadoid == carpeta && c.AreaId == area).ToList();

            var confcarpetadetalle = db.ConfCarpetaDetalle.Where(c => c.CarpetaEncabezadoid == carpeta && c.AreaId == area).ToList();

            foreach (var itemdetalle in confcarpetadetalle)
            {
                string tipocampo = itemdetalle.Tipo_Campo.Descripcion;
                if (itemdetalle.Tipo_Campo.Descripcion == "Fecha")
                {
                    existefecha = 1;
                }
                if (itemdetalle.Tipo_Campo.Descripcion == "Fecha y Hora") { existefecha = 2; }
            }


            if ((existefecha == 1) || (existefecha == 2)) //comienza decodificacion
            {


                lst = (from s in db.Documentos_Detalle
                       where s.llaveUnica.Contains(Nombre) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta
                       select new datos_reporte
                       {
                           Id = s.Id.ToString(),
                           Descripcion_area = s.Areas.Descripcion,
                           carpeta_Descripcion = s.ConfCarpetaEncabezado.Descripcion,
                           llaveUnica = s.llaveUnica,
                           Nombre_Ori = s.Nombre_Ori,
                           Nombre_Des = s.Nombre_Des,
                           FechaRegistro = s.FechaRegistro,
                           FechaActualizacion = s.FechaActualizacion
                       }).ToList();



                foreach (var item2 in lst)
                {
                    string stringllavedescodificada = "";
                    string finalllave = "";
                    stringllavedescodificada = llavedescodificada(carpeta, area, item2.llaveUnica);

                    if (existefecha == 1) //fecha = 8 caracteres ddMMyyyy
                    {
                        if (stringllavedescodificada.Length >= 8)
                        {//= sCadena.substring(5,10);
                            finalllave = stringllavedescodificada.Substring(stringllavedescodificada.Length - 8, 8);
                        }
                        else if (stringllavedescodificada.Length < 8)
                        {
                            finalllave = stringllavedescodificada.Substring(0, stringllavedescodificada.Length);
                        }

                        string fechas = finalllave.Substring(6, 2) + "/" + finalllave.Substring(4, 2) + "/" + finalllave.Substring(0, 4);

                        DateTime fechauser = Convert.ToDateTime(fechas);
                        item2.FechaUsuario = fechauser;
                    }
                    else if (existefecha == 2) //fecha y hora = 14 caracteres ddMMyyyyhhmmss
                    {
                        if (stringllavedescodificada.Length >= 14)
                        {//= sCadena.substring(5,10);
                            finalllave = stringllavedescodificada.Substring(stringllavedescodificada.Length - 14, 14);

                        }
                        else if (stringllavedescodificada.Length < 14)
                        {
                            finalllave = stringllavedescodificada.Substring(0, stringllavedescodificada.Length);
                            int faltan = 0;
                            faltan = 14 - finalllave.Length;
                            for (int i = 1; i <= faltan; i++)
                            { finalllave = finalllave + "0"; }
                        }
                        string fechas = finalllave.Substring(6, 2) + "/" + finalllave.Substring(4, 2) + "/" + finalllave.Substring(0, 4) + " " + finalllave.Substring(8, 2) + ":" + finalllave.Substring(10, 2) + ":" + finalllave.Substring(12, 2);

                        DateTime fechauser = Convert.ToDateTime(fechas);
                        item2.FechaUsuario = fechauser;

                    }
                }


            }
            else
            {
                lst = (from s in db.Documentos_Detalle
                       where s.llaveUnica.Contains(Nombre) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta
                       select new datos_reporte
                       {
                           Id = s.Id.ToString(),
                           Descripcion_area = s.Areas.Descripcion,
                           carpeta_Descripcion = s.ConfCarpetaEncabezado.Descripcion,
                           llaveUnica = s.llaveUnica,
                           Nombre_Ori = s.Nombre_Ori,
                           Nombre_Des = s.Nombre_Des,
                           FechaRegistro = s.FechaRegistro,
                           FechaActualizacion = s.FechaActualizacion,
                           FechaUsuario = s.FechaRegistro
                       }).ToList();
            }




            //if (rname.Contains("JEFE DE FARMACIA"))
            //{
            //    lst = (from s in db.Documentos_Detalle
            //           where s.llaveUnica.Contains(Nombre) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta && s.RoleName == rname
            //           select new datos_reporte
            //           {
            //               Descripcion_area = s.Areas.Descripcion,
            //               carpeta_Descripcion = s.ConfCarpetaEncabezado.Descripcion,
            //               llaveUnica = s.llaveUnica,
            //               Nombre_Ori = s.Nombre_Ori,
            //               Nombre_Des = s.Nombre_Des,
            //               FechaRegistro = s.FechaRegistro,
            //               FechaActualizacion = s.FechaActualizacion
            //           }).ToList();
            //}
            //else
            //{
            //    lst = (from s in db.Documentos_Detalle
            //           where s.llaveUnica.Contains(Nombre) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta
            //           select new datos_reporte
            //           {
            //                Descripcion_area = s.Areas.Descripcion,
            //                carpeta_Descripcion = s.ConfCarpetaEncabezado.Descripcion,
            //               llaveUnica = s.llaveUnica,
            //               Nombre_Ori = s.Nombre_Ori,
            //               Nombre_Des = s.Nombre_Des,
            //               FechaRegistro = s.FechaRegistro,
            //               FechaActualizacion = s.FechaActualizacion}).ToList();
            //}

            if (lst.Count == 0)
            {
                Request.Flash("warning", "¡No se encontraron registros!");
                return RedirectToAction("Index");
            }

            var myExport = new CsvExport();

            foreach (var item in lst)
            {
                myExport.AddRow();
                myExport["ID"] = item.Id;
                myExport["AREA"] = item.Descripcion_area;
                myExport["CARPETA"] = item.carpeta_Descripcion;
                myExport["CODIFICACION"] = item.llaveUnica;
                myExport["NOMBRE ORIGEN"] = item.Nombre_Ori;
                myExport["NOMBRE DESTINO"] = item.Nombre_Des;
                myExport["FECHA SYS"] = item.FechaRegistro;
                myExport["FECHA ACTUALIZACION"] = item.FechaActualizacion;
                myExport["FECHA USUARIO"] = item.FechaUsuario;

            }

            string myCsv = myExport.Export();
            byte[] myCsvData = myExport.ExportToBytes();

            //// Obtiene la respuesta actual
            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;


            // Tipo de contenido para forzar la descarga
            response.ContentType = "application/octet-stream";
            response.AddHeader("Content-Disposition", "attachment; filename=" + "Reporte_de_Documentos.csv");

            // Envia los bytes
            response.BinaryWrite(myCsvData);
            HttpContext.Response.Flush(); // '// Sends all currently buffered output To the client.
            HttpContext.Response.SuppressContent = true; // '// Gets Or sets a value indicating whether To send HTTP content To the client.
            HttpContext.ApplicationInstance.CompleteRequest();

            // Borra la respuesta
            response.Clear();
            response.ClearContent();

            Request.Flash("success", "¡Procesado Satisfactoriamente!");
            return View(lst);
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
