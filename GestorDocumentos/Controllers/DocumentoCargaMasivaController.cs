using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestorDocumentos.Models;
using Newtonsoft.Json;
using System.IO;
using GestorDocumentos.Archivos;
using System.Globalization;
using GestorDocumentos.Clases;
namespace GestorDocumentos.Controllers
{
    public class DocumentoCargaMasivaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public string error;
        // GET: DocumentoCargaMasiva
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            try 
            {
                List<SelectListItem> list = new List<SelectListItem>();

                var usuario = (from u in db.Users
                               where u.UserName == User.Identity.Name
                               select new { u.Id }).ToString();

                DropboxListSubniveles rolenames = new DropboxListSubniveles();
                string rname = rolenames.rolename(User.Identity.Name.ToString());
                var areas = (from a in db.Areas
                             join axr in db.RoleXAreas on a.Id equals axr.AreaId
                             where axr.RoleName == rname
                             select new { a.Id, a.Descripcion }).ToList();
   
                if (areas.Count == 0)
                {
                    var areasxcarpeta = (from a in db.Areas
                                         join axrxc in db.RoleXAreaXCarpetas on
                                         a.Id equals axrxc.AreaId
                                         where axrxc.RoleName == rname
                                         select new { a.Id, a.Descripcion }).Distinct().ToList();
                    if (areasxcarpeta.Count == 0)
                    {
                        var areasxcarpetasxsubniveles = (from a in db.Areas
                                                         join axrxcxs in db.RoleXAreaXCarpetasXSubniveles on
                                                         a.Id equals axrxcxs.AreaId
                                                         where axrxcxs.RoleName == rname
                                                         select new { a.Id, a.Descripcion }).Distinct().ToList();
                        foreach (var item in areasxcarpetasxsubniveles)
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
            }
            catch(Exception ex) 
            {
                Console.Write(ex.Message);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,llaveUnica,AreaId,CarpetaEncabezadoid,Nombre_Ori,Nombre_Des,Imagen,FechaRegistro,FechaActualizacion,RoleName")] Documento_Detalle documento_Detalle, HttpPostedFileBase[] file)
        {
            try 
            {
                byte[] array;
                string extension = "";
                string nombrenuevo = "";
                int existefecha = 0;
                // Agregado: 01052020
                string nombre = "";
                string fila = "";
                string archivo = "";
                long repetidos = 0;
                int longitud = 0;
                string ext = "";
                //*************************************
                int contador = 0;
                foreach (HttpPostedFileBase files in file)
                {
                    if (files != null)
                    {
                        // Agregado: 01052020
                        contador += 1;
                        fila = files.FileName;
                        repetidos = fila.LongCount(letra => letra.ToString() == ".");

                        if (repetidos > 1)
                        {
                            longitud = (fila.Length - 1);
                            longitud = longitud - 3;
                            ext = fila.Substring(longitud, 4);
                            archivo = fila.Substring(0, longitud);
                            archivo = archivo.Replace(".", "");
                            archivo = archivo + ext;
                            nombre = archivo;
                        }
                        else
                        {
                            fila = files.FileName;
                            nombre = files.FileName;
                        }
                        //*************************************

                        //string nombre = file.FileName;
                        nombre = nombre.ToUpper();
                        int posicionpunto = nombre.IndexOf(".");
                        int fin = nombre.Length - 1;

                        try
                        {
                            extension = nombre.Substring(posicionpunto);
                            // extension = nombre.Substring(posicionpunto, fin);
                        }
                        catch (Exception EX)
                        {
                            Console.WriteLine(EX.Message);
                        }

                        if (contador == 1) 
                        {
                            Session["llaveunica"] = documento_Detalle.llaveUnica;
                        }
                        //nombrenuevo = documento_Detalle.llaveUnica + extension;
                        nombrenuevo = Session["llaveunica"] as string;
                        nombrenuevo = nombrenuevo + extension;
                        // ****************************** Valdiación de punto en nombre destino
                        string fila2 = "";
                        string archivo2 = "";
                        long repetidos2 = 0;
                        int longitud2 = 0;
                        string ext2 = "";

                        fila2 = nombrenuevo;
                        repetidos2 = fila2.LongCount(letra => letra.ToString() == ".");

                        if (repetidos2 > 1)
                        {
                            longitud2 = (fila2.Length - 1);
                            longitud2 = longitud2 - 3;
                            ext2 = fila2.Substring(longitud2, 4);
                            archivo2 = fila2.Substring(0, longitud2);
                            archivo2 = archivo2.Replace(".", "");
                            documento_Detalle.llaveUnica = archivo2;
                            archivo2 = archivo2 + ext2;
                            nombrenuevo = archivo2;
                        }
                        else
                        {
                            nombrenuevo = fila2;
                        }
                        //******************************************************************************************

                        using (MemoryStream ms = new MemoryStream())
                        {
                            files.InputStream.CopyTo(ms);
                            //array = ms.GetBuffer();
                            array = ms.ToArray();
                            documento_Detalle.Imagen = array;
                        }

                        documento_Detalle.Nombre_Ori = nombre;
                        documento_Detalle.Nombre_Des = nombrenuevo;

                        //Agregado:15102020
                        DateTime fechaserver = DateTime.Now;
                        string sfechaserver = fechaserver.ToString("dd/MM/yyyy HH:mm:ss");
                        DateTime fechaseteada = Convert.ToDateTime(sfechaserver);
                        documento_Detalle.FechaRegistro = fechaseteada;
                        documento_Detalle.FechaActualizacion = fechaseteada;
                        Session["error"] = fechaseteada;
                        //Agregado:15102020
                        DropboxListSubniveles rolenames = new DropboxListSubniveles();
                        string rname = rolenames.rolename(User.Identity.Name.ToString());
                        documento_Detalle.RoleName = rname;

                        int existeauto = 0;
                        if (documento_Detalle.Nombre_Ori != null)
                        {
                            if (documento_Detalle.Nombre_Des != null)
                            {
                                var carpetadetalle = db.ConfCarpetaDetalle.Where(c => c.CarpetaEncabezadoid == documento_Detalle.CarpetaEncabezadoid).ToList();
                                foreach (var item in carpetadetalle)
                                {
                                    string tipocampo = item.Tipo_Campo.Descripcion;
                                    if (item.Tipo_Campo.Descripcion == "Auto Numérico")
                                    {
                                        existeauto = 1;
                                    }
                                    if (item.Tipo_Campo.Descripcion == "Fecha")
                                    {
                                        existefecha = 1;
                                    }
                                    if (item.Tipo_Campo.Descripcion == "Fecha y Hora")
                                    {
                                        existefecha = 2;
                                    }
                                }

                                db.Documentos_Detalle.Add(documento_Detalle);
                                await db.SaveChangesAsync();
                                if ((existeauto == 1) || (existefecha == 1) || (existefecha == 2))
                                {
                                    string agregados = "";
                                    //if fecha (1) o fecha y hora (2)
                                    if (existefecha == 1) //comentado por el cambio realizado en el JavaScript
                                    {
                                        //Código
                                    }
                                    else if (existefecha == 2)
                                    {
                                        DateTime tiempo = DateTime.Now;
                                        string hora = "";
                                        //string fecha = "";

                                        hora = tiempo.ToString("HHmmss");
                                        //fecha = tiempo.ToString("ddMMyyyy");

                                        agregados = hora;
                                        var utabla = db.Documentos_Detalle.Find(documento_Detalle.Id);
                                        documento_Detalle.llaveUnica= Session["llaveunica"] as string;
                                        utabla.llaveUnica = documento_Detalle.llaveUnica + agregados;
                                        if (extension != "")
                                        {
                                            utabla.Nombre_Des = documento_Detalle.llaveUnica + extension;
                                        }

                                        db.Entry(utabla).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    if (existeauto == 1)
                                    {
                                        var utabla = db.Documentos_Detalle.Find(documento_Detalle.Id);
                                        utabla.llaveUnica = documento_Detalle.llaveUnica + "-" + documento_Detalle.Id;
                                        if (extension != "")
                                        {
                                            utabla.Nombre_Des = documento_Detalle.llaveUnica + extension;
                                        }

                                        db.Entry(utabla).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                }
                            }
                        }
                    }
                }
                Request.Flash("success", "Documento agregado correctamente");
                return RedirectToAction("create");
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", documento_Detalle.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", documento_Detalle.CarpetaEncabezadoid);
            return View(documento_Detalle);
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

            var mn = db.MantenimientoSubniveles.OrderBy(x => x.Descripcion).Where(ms => ms.CarpetaEncabezadoid == id);

            if (Efarmacia.Count > 0)
            {
                int numero = Efarmacia[0];
                //mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero));
                mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6") && !ms.Descripcion.Contains("H0") && !ms.Descripcion.Contains("H1") && !ms.Descripcion.Contains("H2") && !ms.Descripcion.Contains("H5") && !ms.Descripcion.Contains("H6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero));
            }

            //var mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id);

            //anterior
            //var selects = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id).Select(n => new { n.ConfCarpetaDetalle.Descripcion, n.ConfCarpetaDetalleId } ).Distinct().ToList();
            var query = (from unionccpms in
                           (from ms in db.MantenimientoSubniveles
                            join ccd in db.ConfCarpetaDetalle on ms.CarpetaEncabezadoid equals ccd.CarpetaEncabezadoid

                            where ms.CarpetaEncabezadoid == id && ms.AreaId == ccd.AreaId && ccd.CarpetaEncabezadoid == id && ms.ConfCarpetaDetalleId == ccd.Id
                            select new { ms.ConfCarpetaDetalle.Descripcion, ms.ConfCarpetaDetalleId, ccd.NivelId, ccd.CarpetaEncabezadoid })
                         where unionccpms.CarpetaEncabezadoid == id
                         orderby unionccpms.NivelId
                         select new { unionccpms.Descripcion, unionccpms.ConfCarpetaDetalleId }
           ).Distinct().ToList();

            var tt = JsonConvert.SerializeObject(mn.ToList());
            var IsFecha = db.ConfCarpetaDetalle.Where(fs => fs.CarpetaEncabezadoid == id).Select(tc => new { tc.Tipo_Campo.Descripcion, tc.Tipo_campoId }).Where(tc => tc.Descripcion == "Fecha").ToList();
            var IsFechayhora = db.ConfCarpetaDetalle.Where(fs => fs.CarpetaEncabezadoid == id).Select(tc => new { tc.Tipo_Campo.Descripcion, tc.Tipo_campoId }).Where(tc => tc.Descripcion == "Fecha y Hora").ToList();

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
            //lo anterior ***********************************
            //return Json(new { options = JsonConvert.SerializeObject(mn.ToList()), selects = JsonConvert.SerializeObject(selects), fecha = JsonConvert.SerializeObject(fecha) });
            //***********************************************
            return Json(new { options = JsonConvert.SerializeObject(mn.ToList()), selects = JsonConvert.SerializeObject(query), fecha = JsonConvert.SerializeObject(fecha) });
        }
        public JsonResult GetAreayEncabezadoCarpetaId(int idarea)
        {
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaareas = subniveles.GetAreayEncabezadoCarpetaIdpermisos(idarea, rname);
            return Json(listaareas, JsonRequestBehavior.AllowGet);
        }

        // Permisos de pantallas
        public JsonResult GetPermisosConsulta(string pantalla)
        {
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.consultar
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermisosCrear(string pantalla)
        {
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.crear
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermisosEditar(string pantalla)
        {
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.editar
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermisosEliminar(string pantalla)
        {
            DropboxListSubniveles rolenames = new DropboxListSubniveles();
            string rname = rolenames.rolename(User.Identity.Name.ToString());

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.eliminar
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFechaRegistro(int CarpetaEncabezadoid, int Id, int AreaId)
        {
            //var doc_creado = db.Documentos_Detalle.Where(c => c.CarpetaEncabezadoid == CarpetaEncabezadoid && c.llaveUnica == llaveUnica && c.AreaId == AreaId).ToList();
            List<SelectListItem> doc_creado = (from d in db.Documentos_Detalle
                                               where d.CarpetaEncabezadoid == CarpetaEncabezadoid && d.Id == Id && d.AreaId == AreaId
                                               select new SelectListItem
                                               {
                                                   Value = d.Id.ToString(),
                                                   Text = d.FechaRegistro.ToString()
                                               }).ToList();
            return Json(doc_creado, JsonRequestBehavior.AllowGet);
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
    }
}