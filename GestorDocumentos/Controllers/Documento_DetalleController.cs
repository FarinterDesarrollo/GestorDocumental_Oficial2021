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
using GestorDocumentos.Servicios;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class Documento_DetalleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly FarmaciasServices _services = new FarmaciasServices();
        public string error;
        public ActionResult Imagen(int id)
        {
            //int id = 2;

            var context = new Models.ApplicationDbContext();
            string nombre= context.Documentos_Detalle.FirstOrDefault(i => i.Id == id)?.Nombre_Des;
            int posicionpunto = nombre.IndexOf(".");
            string extension = (nombre.Substring(posicionpunto)).ToUpper();
            byte[] imagedata = context.Documentos_Detalle.FirstOrDefault(i => i.Id == id)?.Imagen;
            if (imagedata != null)
            {
                if (extension == ".PNG")
                {
                    return File(imagedata, "image/png");                                                                  
                }
                if (extension == ".JPG" || extension==".JPEG")
                {
                    return File(imagedata, "image/jpg");
                }
                if (extension == ".PDF")
                {
                    return File(imagedata, "application/pdf");
                }
                if (extension == ".XLSX" || extension==".XLS" || extension==".CSV")
                {
                    return File(imagedata, "application/octet-stream",nombre);
                }
                if (extension==".DOCX" || extension==".DOC")
                {
                    return File(imagedata, "application/octet-stream", nombre);
                }
                
                //return File(imagedata, "image/png");
            }
            return null;
        }

        // GET: Documento_Detalle
        //public async Task<ActionResult> Index()
    public  ActionResult Index()
        {
            //DropboxListSubniveles rolenames = new DropboxListSubniveles();
            //string rname = rolenames.rolename(User.Identity.Name.ToString());
            ////List<SelectListItem> documentos_Detalle = new List<SelectListItem>();
            //var areasxcarpetas = (from a in db.Areas
            //                      join rxcxa in db.RoleXAreaXCarpetas on a.Id equals rxcxa.AreaId
            //                      join dd in db.Documentos_Detalle on rxcxa.CarpetaEncabezadoid equals dd.CarpetaEncabezadoid
            //                      join cc in db.ConfCarpetaEncabezados on dd.CarpetaEncabezadoid equals cc.Id
            //                      where a.Id == dd.AreaId && rxcxa.RoleName == rname && cc.AreaId == dd.AreaId && cc.AreaId == a.Id && rxcxa.AreaId == cc.AreaId &&
            //                      rxcxa.AreaId == dd.AreaId && dd.RoleName ==rname
            //                      select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
            //                      ).ToList();
            ////var documentos_Detalle;
            //if (areasxcarpetas.Count == 0)
            //{
            //    var areasxrole = (from a in db.Areas
            //                      join rxa in db.RoleXAreas on a.Id equals rxa.AreaId
            //                      join dd in db.Documentos_Detalle on rxa.AreaId equals dd.AreaId
            //                      join cc in db.ConfCarpetaEncabezados on a.Id equals cc.AreaId
            //                      where rxa.RoleName == rname && cc.AreaId == dd.AreaId && rxa.AreaId == cc.AreaId && cc.Id == dd.CarpetaEncabezadoid && dd.RoleName == rname
            //                      select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro,dd.FechaActualizacion }
            //                      ).ToList();
            //    if (areasxrole.Count == 0)
            //    {
            //        var areasxrolexsubnivel = (from a in db.Areas
            //                                   join rxaxs in db.RoleXAreaXCarpetasXSubniveles on a.Id equals rxaxs.AreaId
            //                                   join dd in db.Documentos_Detalle on rxaxs.AreaId equals dd.AreaId
            //                                   join cc in db.ConfCarpetaEncabezados on a.Id equals cc.AreaId
            //                                   where rxaxs.RoleName == rname && cc.AreaId == dd.AreaId && rxaxs.AreaId == cc.AreaId && cc.Id == dd.CarpetaEncabezadoid && dd.RoleName == rname
            //                                   select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
            //                      ).Distinct().ToList();
            //        List<DocDetPermisos> documentos_Detalle;
            //        documentos_Detalle = (from s in areasxrolexsubnivel
            //                              select new DocDetPermisos
            //                              {
            //                                  Id = s.Id,
            //                                  area = s.Descripcion,
            //                                  carpeta = s.carpeta,
            //                                  llaveunica = s.llaveUnica,
            //                                  Nombreori = s.Nombre_Ori,
            //                                  Nombredes = s.Nombre_Des,
            //                                  FechaRegistro= s.FechaRegistro,
            //                                  FechaActualizacion = s.FechaActualizacion
            //                              }).ToList();

            //        //string FechaR = documentos_Detalle[0].FechaRegistro.ToString("dd/MM/yyyy 00:00:00", CultureInfo.InvariantCulture);
            //        //string FechaA = documentos_Detalle[0].FechaActualizacion.ToString("dd/MM/yyyy 00:00:00", CultureInfo.InvariantCulture);

            //        //documentos_Detalle[0].FechaRegistro = Convert.ToDateTime(FechaR);
            //        //documentos_Detalle[0].FechaActualizacion = Convert.ToDateTime(FechaA);

            //        return View(documentos_Detalle.ToList());
            //    }
            //    else
            //    {
            //        List<DocDetPermisos> documentos_Detalle;
            //        documentos_Detalle = (from s in areasxrole
            //                              select new DocDetPermisos
            //                              {
            //                                  Id = s.Id,
            //                                  area = s.Descripcion,
            //                                  carpeta = s.carpeta,
            //                                  llaveunica = s.llaveUnica,
            //                                  Nombreori = s.Nombre_Ori,
            //                                  Nombredes = s.Nombre_Des,
            //                                  FechaRegistro=s.FechaRegistro,
            //                                  FechaActualizacion=s.FechaActualizacion

            //                              }).ToList();
            //        return View(documentos_Detalle.ToList());
            //    }
            //}
            //else
            //{
            //    List<DocDetPermisos> documentos_Detalle;
            //    documentos_Detalle = (from s in areasxcarpetas
            //                          select new DocDetPermisos
            //                          {
            //                              Id = s.Id,
            //                              area = s.Descripcion,
            //                              carpeta = s.carpeta,
            //                              llaveunica = s.llaveUnica,
            //                              Nombreori = s.Nombre_Ori,
            //                              Nombredes = s.Nombre_Des,
            //                              FechaRegistro=s.FechaRegistro,
            //                              FechaActualizacion=s.FechaActualizacion

            //                          }).ToList();
            //    return View(documentos_Detalle.ToList());
            //}
            /// Hasta aqui arriba

            // //List<ListTablaViewModel> lst;
            // //using (CrudEntities db = new CrudEntities())
            // //{
            // //    lst = (from d in db.tabla
            // //           select new ListTablaViewModel
            // //           {
            // //               Id = d.Id,
            // //               nombre = d.nombre,
            // //               correo = d.correo
            // //           }).ToList();
            // //}
            // //return View(lst);
            // DropboxListSubniveles rolenames = new DropboxListSubniveles();
            // string rname = rolenames.rolename(User.Identity.Name.ToString());
            // //List<SelectListItem> documentos_Detalle = new List<SelectListItem>();
            // var areasxcarpetas = (from a in db.Areas 
            //                       join rxcxa in db.RoleXAreaXCarpetas on a.Id equals rxcxa.AreaId
            //                       join dd in db.Documentos_Detalle on rxcxa.CarpetaEncabezadoid equals dd.CarpetaEncabezadoid
            //                       join cc in db.ConfCarpetaEncabezados on dd.CarpetaEncabezadoid equals cc.Id 
            //                       where a.Id == dd.AreaId &&  rxcxa.RoleName==rname && cc.AreaId==dd.AreaId && cc.AreaId==a.Id && rxcxa.AreaId==cc.AreaId &&
            //                       rxcxa.AreaId==dd.AreaId 
            //                       select new { dd.Id, a.Descripcion, carpeta=cc.Descripcion,dd.llaveUnica,dd.Nombre_Ori,dd.Nombre_Des}
            //                       ).ToList();
            // //var documentos_Detalle;
            //if (areasxcarpetas.Count==0)
            // {
            //     var areasxrole=(from a in db.Areas
            //                     join rxa in db.RoleXAreas on a.Id equals rxa.AreaId
            //                     join dd in db.Documentos_Detalle on rxa.AreaId equals dd.AreaId
            //                     join cc in db.ConfCarpetaEncabezados on a.Id equals cc.AreaId 
            //                     where rxa.RoleName==rname && cc.AreaId==dd.AreaId && rxa.AreaId==cc.AreaId && cc.Id ==dd.CarpetaEncabezadoid
            //                     select new { dd.Id,a.Descripcion, carpeta=cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des }
            //                       ).ToList();

            //     List<DocDetPermisos> documentos_Detalle;
            //     documentos_Detalle = (from s in areasxrole
            //                           select new DocDetPermisos
            //                           {
            //                               Id = s.Id,
            //                               area = s.Descripcion,
            //                               carpeta = s.carpeta,
            //                               llaveunica = s.llaveUnica,
            //                               Nombreori = s.Nombre_Ori,
            //                               Nombredes = s.Nombre_Des

            //                           }).ToList();
            //     return View(documentos_Detalle.ToList());

            // }
            // else
            // {
            //     List<DocDetPermisos> documentos_Detalle;
            //     documentos_Detalle = (from s in areasxcarpetas
            //                           select new DocDetPermisos
            //                           {
            //                               Id = s.Id,
            //                               area = s.Descripcion,
            //                               carpeta = s.carpeta,
            //                               llaveunica = s.llaveUnica,
            //                               Nombreori = s.Nombre_Ori,
            //                               Nombredes = s.Nombre_Des

            //                           }).ToList();
            //     return View(documentos_Detalle.ToList());
            // }


            // //var documentos_Detalle = db.Documentos_Detalle.Include(d => d.Areas).Include(d => d.ConfCarpetaEncabezado);
            // //return View(await documentos_Detalle.ToListAsync());
            //ViewBag.Error = Session["error"] as string;
            return View();
        }

        // GET: Documento_Detalle/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);
            if (documento_Detalle == null)
            {
                return HttpNotFound();
            }
            return View(documento_Detalle);
        }

        // GET: Documento_Detalle/Create
        public ActionResult Create()
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


            return View();

            //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion");
            //ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion");
            //return View();

            //List<SelectListItem> list = new List<SelectListItem>();

            //var usuario = (from u in db.Users
            //               where u.UserName == User.Identity.Name
            //               select new { u.Id }).ToString();

            //DropboxListSubniveles rolenames = new DropboxListSubniveles();
            //string rname = rolenames.rolename(User.Identity.Name.ToString());
            //var areas = (from a in db.Areas
            //             join axr in db.RoleXAreas on a.Id equals axr.AreaId
            //             where axr.RoleName == rname
            //             select new { a.Id, a.Descripcion }).ToList();

            //if (areas.Count == 0)
            //{
            //    var areasxcarpeta = (from a in db.Areas
            //                         join axrxc in db.RoleXAreaXCarpetas on
            //                         a.Id equals axrxc.AreaId
            //                         where axrxc.RoleName == rname
            //                         select new { a.Id, a.Descripcion }).Distinct().ToList();
            //    foreach (var item in areasxcarpeta)
            //    {
            //        list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
            //    }

            //}
            //else
            //{
            //    foreach (var item in areas)
            //    {
            //        list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Descripcion });
            //    }
            //}

            //if (list.Count == 0)
            //{
            //    ViewBag.AreaId = list;
            //    ViewBag.CarpetaEncabezadoid = list;
            //}
            //else
            //{
            //    ViewBag.AreaId = list;
            //    //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion");
            //    ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion");
            //}


            //return View();

            ////ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion");
            ////ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion");
            ////return View();
        }

        public class listafecha
        {
            public int Campo_TipoId { get; set; }
            public string Descripcion { get; set; }
            public int Tipo { get; set; }
        }

        [HttpPost]
        public JsonResult fields(int id) {
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

            //IQueryable<MantenimientoSubniveles> mn;
            var mn = db.MantenimientoSubniveles.OrderBy(x => x.Descripcion).Where(ms => ms.CarpetaEncabezadoid == id);

            if (Efarmacia.Count > 0)
            {
                int numero = Efarmacia[0];
                //var subnivel= db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && (ms.Descripcion.Contains("K0") || ms.Descripcion.Contains("K1") || ms.Descripcion.Contains("K2") || ms.Descripcion.Contains("K5") || ms.Descripcion.Contains("K6"))).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && (ms.Descripcion.Contains("H0") || ms.Descripcion.Contains("H1") || ms.Descripcion.Contains("H2") || ms.Descripcion.Contains("H5") || ms.Descripcion.Contains("H6")))).ToList();
                var subnivel = _services.MS_Farmacias_PermisosLista(id, numero, 1);
                if (subnivel.Count > 0) 
                {
                    //mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6") && !ms.Descripcion.Contains("H0") && !ms.Descripcion.Contains("H1") && !ms.Descripcion.Contains("H2") && !ms.Descripcion.Contains("H5") && !ms.Descripcion.Contains("H6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero)); //original 20211112
                    mn = _services.MS_Farmacias_Permisos(id, numero, 2);
                }
                else 
                {
                    mn = (from ms in db.MantenimientoSubniveles join rxaxcxs in db.RoleXAreaXCarpetasXSubniveles
                              on ms.CarpetaEncabezadoid equals rxaxcxs.CarpetaEncabezadoid
                              where ms.Id == rxaxcxs.MantenimientoSubnivelesid && rxaxcxs.RoleName == rname && ms.CarpetaEncabezadoid == id
                              select ms);

                    // Modificado --> 20211112
                }
                //mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero));
            }
            else 
            {
                mn = db.MantenimientoSubniveles.OrderBy(x => x.Descripcion).Where(ms => ms.CarpetaEncabezadoid == id);
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
            var IsFecha = db.ConfCarpetaDetalle.Where(fs => fs.CarpetaEncabezadoid == id).Select(tc => new { tc.Tipo_Campo.Descripcion, tc.Tipo_campoId}).Where(tc => tc.Descripcion == "Fecha" ).ToList();
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
            return Json(new { options = JsonConvert.SerializeObject(mn.ToList()), selects = JsonConvert.SerializeObject(query),fecha=JsonConvert.SerializeObject(fecha)});
        }

        // POST: Documento_Detalle/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,llaveUnica,AreaId,CarpetaEncabezadoid,Nombre_Ori,Nombre_Des,Imagen,FechaRegistro,FechaActualizacion,RoleName")] Documento_Detalle documento_Detalle, HttpPostedFileBase file)
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

            if (file != null)
            {
                // Agregado: 01052020
                fila = file.FileName;
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
                    fila = file.FileName;
                    nombre = file.FileName;
                }
                //*************************************

                //string nombre = file.FileName;
                nombre = nombre.ToUpper();
                int posicionpunto = nombre.IndexOf(".");
                int fin = nombre.Length-1 ;
              
                try
                {
                    extension = nombre.Substring(posicionpunto);
                   // extension = nombre.Substring(posicionpunto, fin);
                }
               catch (Exception EX)
                {

                }

                nombrenuevo = documento_Detalle.llaveUnica + extension;
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

                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                    documento_Detalle.Imagen = array;
                }

                documento_Detalle.Nombre_Ori = nombre;
                documento_Detalle.Nombre_Des = nombrenuevo;
                //Agregado:14042020
                //var dateQuery = db.Database.SqlQuery<DateTime>("SELECT  now()");
                //var dateQuery = db.Database.SqlQuery<string>("select TO_CHAR(now(), 'DD/MM/YYYY HH24:MI:SS')");
                //Session["error"] = Convert.ToString(dateQuery.AsEnumerable().First());
                ////DateTime serverDate = Convert.ToDateTime(dateQuery.AsEnumerable().First());
                //DateTime serverDate = Convert.ToDateTime(Convert.ToString(dateQuery.AsEnumerable().First()));
                ////DateTime FechaR = DateTime.Today;
                //DateTime FechaR = serverDate;
                //documento_Detalle.FechaRegistro = FechaR;
                //documento_Detalle.FechaActualizacion = FechaR;
                //Agregado:06052020

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
            }

            int existeauto = 0;
            if (documento_Detalle.Nombre_Ori != null)    
            {
                if (documento_Detalle.Nombre_Des != null)
                {
                    var carpetadetalle = db.ConfCarpetaDetalle.Where(c => c.CarpetaEncabezadoid == documento_Detalle.CarpetaEncabezadoid).ToList();
                    foreach (var item  in carpetadetalle)
                    {
                        string tipocampo = item.Tipo_Campo.Descripcion;
                        if (item.Tipo_Campo.Descripcion== "Auto Numérico")
                        {
                            existeauto=1;
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
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        goto error;
                    }

                    if ((existeauto == 1) || (existefecha ==1) ||(existefecha==2 ))
                    {
                        string agregados = "";
                        //if fecha (1) o fecha y hora (2)
                        if (existefecha == 1) //comentado por el cambio realizado en el JavaScript
                        {
                            ////hora = tiempo.ToString("hhmmss");
                            ////fecha = tiempo.ToString("ddMMyyyy");
                            //DateTime tiempo = DateTime.Now;
                            //agregados = tiempo.ToString("ddMMyyyy");
                            //var utabla = db.Documentos_Detalle.Find(documento_Detalle.Id);
                            //utabla.llaveUnica = documento_Detalle.llaveUnica + "-" + agregados;
                            //if (extension != "")
                            //{
                            //    utabla.Nombre_Des = documento_Detalle.llaveUnica + extension;
                            //}

                            //db.Entry(utabla).State = System.Data.Entity.EntityState.Modified;
                            //db.SaveChanges();


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
                            utabla.llaveUnica = documento_Detalle.llaveUnica + agregados;
                            if (extension != "")
                            {
                                utabla.Nombre_Des = documento_Detalle.llaveUnica + extension;
                            }

                            db.Entry(utabla).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }

                         if (existeauto==1)
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
                    Request.Flash("success", "Documento agregado correctamente");
                    return RedirectToAction("Index");
                }
            }

            //if (ModelState.IsValid)
            //{
            //    db.Documentos_Detalle.Add(documento_Detalle);
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}
            error:
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", documento_Detalle.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", documento_Detalle.CarpetaEncabezadoid);
            return View(documento_Detalle);
        }

        // GET: Documento_Detalle/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);

            if (documento_Detalle == null)
            {
                return HttpNotFound();
            }

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


            return View();

            //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", documento_Detalle.AreaId);
            //ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", documento_Detalle.CarpetaEncabezadoid);
            //return View(documento_Detalle);
        }

        // POST: Documento_Detalle/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,llaveUnica,AreaId,CarpetaEncabezadoid,Nombre_Ori,Nombre_Des,Imagen,FechaRegistro,FechaActualizacion,RoleName")] Documento_Detalle documento_Detalle, HttpPostedFileBase file, int NCarpetaEncabezadoid)
        {
            byte[] array;
            string nombrenuevo = "";
            string extension = "";
            int existefecha = 0;
            // Agregado: 01052020
            string nombre = "";
            string fila = "";
            string archivo = "";
            long repetidos = 0;
            int longitud = 0;
            string ext = "";
            //*************************************

            //Agregado 23/09/2020:
            documento_Detalle.CarpetaEncabezadoid = NCarpetaEncabezadoid;
            //*************************************

            if (file != null)
            {
                //string nombre = file.FileName;

                // Agregado: 01052020
                fila = file.FileName;
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
                    fila = file.FileName;
                    nombre = file.FileName;
                }
                //*************************************
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

                }

                nombrenuevo = documento_Detalle.llaveUnica + extension;

                using (MemoryStream ms = new MemoryStream())
                {

                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                    documento_Detalle.Imagen = array;
                }

                documento_Detalle.Nombre_Ori = nombre;
                documento_Detalle.Nombre_Des = nombrenuevo;
                //Agregado:14042020
                var dateQuery = db.Database.SqlQuery<string>("select TO_CHAR(now(), 'DD/MM/YYYY HH24:MI:SS')");
                DateTime serverDate = Convert.ToDateTime(dateQuery.AsEnumerable().First());
                //DateTime FechaA = DateTime.Today;
                DateTime FechaA = serverDate;
                DateTime FechaR = documento_Detalle.FechaRegistro;
                documento_Detalle.FechaActualizacion = FechaA;
                documento_Detalle.FechaRegistro = FechaR;
                //Agregado:06052020
                DropboxListSubniveles rolenames = new DropboxListSubniveles();
                string rname = rolenames.rolename(User.Identity.Name.ToString());
                documento_Detalle.RoleName = rname;
            }
            else
            {
                var context = new Models.ApplicationDbContext();
                byte[] imagen = context.Documentos_Detalle.FirstOrDefault(i => i.Id == documento_Detalle.Id)?.Imagen;
                documento_Detalle.Imagen = imagen;
                // nombrenuevo = documento_Detalle.Nombre_Des;
            }

            int existeauto = 0;
            if (documento_Detalle.Nombre_Ori != null)
            {
                if (documento_Detalle.Nombre_Des != null)
                {
                    if (documento_Detalle.Imagen != null)
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

                        db.Entry(documento_Detalle).State = EntityState.Modified;
                        await db.SaveChangesAsync();

                        if ((existeauto == 1) || (existefecha == 1) || (existefecha == 2))
                        {
                            string agregados = "";
                            //if fecha (1) o fecha y hora (2)
                            if (existefecha == 1)
                            {
                                //hora = tiempo.ToString("hhmmss");
                                //fecha = tiempo.ToString("ddMMyyyy");
                                //DateTime tiempo = DateTime.Now;
                                //agregados = tiempo.ToString("ddMMyyyy");
                                //var utabla = db.Documentos_Detalle.Find(documento_Detalle.Id);
                                //utabla.llaveUnica = documento_Detalle.llaveUnica + "-" + agregados;
                                //if (extension != "")
                                //{
                                //    utabla.Nombre_Des = documento_Detalle.llaveUnica + extension;
                                //}

                                //db.Entry(utabla).State = System.Data.Entity.EntityState.Modified;
                                //db.SaveChanges();


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
                Request.Flash("success", "Documento actualizado correctamente");
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", documento_Detalle.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", documento_Detalle.CarpetaEncabezadoid);
            return View(documento_Detalle);
        }

        // GET: Documento_Detalle/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);
            if (documento_Detalle == null)
            {
                return HttpNotFound();
            }
            return View(documento_Detalle);
        }

        // POST: Documento_Detalle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);
            db.Documentos_Detalle.Remove(documento_Detalle);
            await db.SaveChangesAsync();
            Request.Flash("success", "Documento eliminado correctamente");
            return RedirectToAction("Index");
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
                                        Value=x.id.ToString(),
                                        Text=x.consultar
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
                                                   Value=d.Id.ToString(),
                                                   Text=d.FechaRegistro.ToString()
                                               }).ToList();
            return Json(doc_creado, JsonRequestBehavior.AllowGet);
        }
        //*******************************************Para generar reporte*******************************************
        public ActionResult GenerarReporte(string ValPrint, int Area, int Encabezadoid)
        {
            ValPrint = ValPrint.ToUpper();
            int area = Area;
            int carpeta = Encabezadoid;
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
                       where s.llaveUnica.Contains(ValPrint) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta
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
                       where s.llaveUnica.Contains(ValPrint) && s.AreaId == area && s.CarpetaEncabezadoid == carpeta
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

            if (lst.Count == 0)
            {
                Session["docEncontro"] = "N";
                return RedirectToAction("Index", "VisualizacionConFiltros");
            }
            else
            {
                Session["docEncontro"] = "Y";
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

            string encontro = Session["docEncontro"] as string;
            if (encontro == "Y")
            {
                Session["docEncontro"] = "";
                ViewBag.Message = "¡Procesado Satisfactoriamente!";
            }
            else
            {
                Session["docEncontro"] = "";
            }
            return View(lst);
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

        //**********************************************************************************************************
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
