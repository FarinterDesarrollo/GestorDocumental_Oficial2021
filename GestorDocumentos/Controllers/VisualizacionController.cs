using GestorDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using GestorDocumentos.Archivos;
using GestorDocumentos.Servicios;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class VisualizacionController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        // GET: Visualizacion
        private readonly FarmaciasServices _services = new FarmaciasServices();

        public VisualizacionController()
        {
        }

        public VisualizacionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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


        public ActionResult Index(string Nombre)
        {
            string valor = Session["llaveUnica"] as string;
            Boolean GTNIC = false;

            if (valor != null && valor !="")
            {
                Nombre = valor;
                Session["llaveUnica"] = "";
            }

            if(Nombre != null)
            {
                bool isNumber = int.TryParse(Nombre, out int number2);
                if (isNumber == false)
                {
                    Nombre = Nombre.ToLower();
                }
            }

            if (!String.IsNullOrEmpty(Nombre))
            {
                
                DropboxListSubniveles rolenames = new DropboxListSubniveles();
                string rname = rolenames.rolename(User.Identity.Name.ToString());

                //List<SelectListItem> documentos_Detalle = new List<SelectListItem>();
                var areasxcarpetas = (from a in db.Areas
                                      join rxcxa in db.RoleXAreaXCarpetas on a.Id equals rxcxa.AreaId
                                      join dd in db.Documentos_Detalle on rxcxa.CarpetaEncabezadoid equals dd.CarpetaEncabezadoid
                                      join cc in db.ConfCarpetaEncabezados on dd.CarpetaEncabezadoid equals cc.Id
                                      where a.Id == dd.AreaId && rxcxa.RoleName == rname && cc.AreaId == dd.AreaId && cc.AreaId == a.Id && rxcxa.AreaId == cc.AreaId &&
                                      rxcxa.AreaId == dd.AreaId
                                      select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
                                      ).ToList();

                // ****************** Validación subniveles GT y NIC
                var rolGTxNIC = (from r in db.Roles select new { r.Name}).Where(x=>x.Name.Contains("COORDINADOR H") || (x.Name.StartsWith("JEFE DE FARMACIA") && x.Name.EndsWith("NIC"))).OrderBy(y=>y.Name).ToList();

                foreach(var item in rolGTxNIC) 
                {
                    if (item.Name == rname)
                    {
                        areasxcarpetas.Clear();
                        GTNIC = true;
                    }
                }

                // *************************************************
                string tipo = _services.VerificarTipoRol(rname);

                //var documentos_Detalle;
                if (areasxcarpetas.Count == 0 || tipo == "A")
                {
                    var areasxrole = (from a in db.Areas
                                      join rxa in db.RoleXAreas on a.Id equals rxa.AreaId
                                      join dd in db.Documentos_Detalle on rxa.AreaId equals dd.AreaId
                                      join cc in db.ConfCarpetaEncabezados on a.Id equals cc.AreaId
                                      where rxa.RoleName == rname && cc.AreaId == dd.AreaId && rxa.AreaId == cc.AreaId && cc.Id == dd.CarpetaEncabezadoid
                                      select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
                                      ).ToList();

                    // ***************** Validación GT y NIC
                    if (GTNIC == true) 
                    {
                        areasxrole.Clear();
                    }
                    // *********************************** FIN

                    if (areasxrole.Count == 0)
                    {
                        var areasxcarpetasxsubniveles = (from a in db.Areas
                                                         join rxcxaxs in db.RoleXAreaXCarpetasXSubniveles on a.Id equals rxcxaxs.AreaId
                                                         join dd in db.Documentos_Detalle on rxcxaxs.CarpetaEncabezadoid equals dd.CarpetaEncabezadoid
                                                         join cc in db.ConfCarpetaEncabezados on dd.CarpetaEncabezadoid equals cc.Id
                                                         where a.Id == dd.AreaId && rxcxaxs.RoleName == rname && cc.AreaId == dd.AreaId && cc.AreaId == a.Id && rxcxaxs.AreaId == cc.AreaId &&
                                                         rxcxaxs.AreaId == dd.AreaId && dd.RoleName == rname
                                                         select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
                                      ).Distinct().ToList();

                        List<DocDetPermisos> documentos_Detalle;
                        string busqueda = Nombre;

                        documentos_Detalle = (from s in areasxcarpetasxsubniveles
                                              select new DocDetPermisos
                                              {
                                                  Id = s.Id,
                                                  area = s.Descripcion,
                                                  carpeta = s.carpeta,
                                                  llaveunica = s.llaveUnica,
                                                  Nombreori = s.Nombre_Ori,
                                                  Nombredes = s.Nombre_Des,
                                                  FechaRegistro = s.FechaRegistro,
                                                  FechaActualizacion = s.FechaActualizacion

                                              }).Where(s => s.Id.ToString().Contains(busqueda) || s.llaveunica.ToLower().Contains(busqueda) || s.area.ToLower().Contains(busqueda) || s.carpeta.ToLower().Contains(busqueda) || s.Nombreori.ToLower().Contains(busqueda) || s.Nombredes.ToLower().Contains(busqueda)).ToList();

                        //.Where(s => s.llaveunica.Contains(Nombre)).ToList();
                        ViewBag.BusqCant = "Coincidencias Encontradas:" + documentos_Detalle.Count;
                        return View(documentos_Detalle.ToList());
                    }
                    else
                    {
                        List<DocDetPermisos> documentos_Detalle;
                        string busqueda = Nombre;

                        documentos_Detalle = (from s in areasxrole
                                              select new DocDetPermisos
                                              {
                                                  Id = s.Id,
                                                  area = s.Descripcion,
                                                  carpeta = s.carpeta,
                                                  llaveunica = s.llaveUnica,
                                                  Nombreori = s.Nombre_Ori,
                                                  Nombredes = s.Nombre_Des,
                                                  FechaRegistro = s.FechaRegistro,
                                                  FechaActualizacion = s.FechaActualizacion

                                              }).Where(s => s.Id.ToString().Contains(busqueda) || s.llaveunica.ToLower().Contains(busqueda) || s.area.ToLower().Contains(busqueda) || s.carpeta.ToLower().Contains(busqueda) || s.Nombreori.ToLower().Contains(busqueda) || s.Nombredes.ToLower().Contains(busqueda)).ToList();
                        //.Where(s => s.llaveunica.Contains(Nombre)).ToList();
                        ViewBag.BusqCant = "Coincidencias Encontradas:" + documentos_Detalle.Count; 
                        return View(documentos_Detalle.ToList());
                    }


                }
                else
                {
                    List<DocDetPermisos> documentos_Detalle;
                    string busqueda = Nombre;
                    documentos_Detalle = (from s in areasxcarpetas
                                          select new DocDetPermisos
                                          {
                                              Id = s.Id,
                                              area = s.Descripcion,
                                              carpeta = s.carpeta,
                                              llaveunica = s.llaveUnica,
                                              Nombreori = s.Nombre_Ori,
                                              Nombredes = s.Nombre_Des,
                                              FechaRegistro = s.FechaRegistro,
                                              FechaActualizacion = s.FechaActualizacion

                                          }).Where(s => s.Id.ToString().Contains(busqueda) || s.llaveunica.ToLower().Contains(busqueda) || s.area.ToLower().Contains(busqueda) || s.carpeta.ToLower().Contains(busqueda) || s.Nombreori.ToLower().Contains(busqueda) || s.Nombredes.ToLower().Contains(busqueda)).ToList();
                    //documentos_Detalle = documentos_Detalle.Where(s => s.llaveunica.Contains(Nombre));
                    //documentos_Detalle = documentos_Detalle.Where(s => s.llaveunica.Contains(Nombre));
                    //Agregado el 16/09/2020:
                    ViewBag.BusqCant = "Coincidencias Encontradas:" + documentos_Detalle.Count;
                    return View(documentos_Detalle.ToList());
                }

                // busqueda = busqueda.Where(s => s.llaveUnica.Contains(Nombre));
            }

            else
            {
                //DropboxListSubniveles rolenames = new DropboxListSubniveles();
                //string rname = rolenames.rolename(User.Identity.Name.ToString());
                ////List<SelectListItem> documentos_Detalle = new List<SelectListItem>();
                //var areasxcarpetas = (from a in db.Areas
                //                      join rxcxa in db.RoleXAreaXCarpetas on a.Id equals rxcxa.AreaId
                //                      join dd in db.Documentos_Detalle on rxcxa.CarpetaEncabezadoid equals dd.CarpetaEncabezadoid
                //                      join cc in db.ConfCarpetaEncabezados on dd.CarpetaEncabezadoid equals cc.Id
                //                      where a.Id == dd.AreaId && rxcxa.RoleName == rname && cc.AreaId == dd.AreaId && cc.AreaId == a.Id && rxcxa.AreaId == cc.AreaId &&
                //                      rxcxa.AreaId == dd.AreaId
                //                      select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
                //                      ).ToList();
                ////var documentos_Detalle;
                //if (areasxcarpetas.Count == 0)
                //{
                //    var areasxrole = (from a in db.Areas
                //                      join rxa in db.RoleXAreas on a.Id equals rxa.AreaId
                //                      join dd in db.Documentos_Detalle on rxa.AreaId equals dd.AreaId
                //                      join cc in db.ConfCarpetaEncabezados on a.Id equals cc.AreaId
                //                      where rxa.RoleName == rname && cc.AreaId == dd.AreaId && rxa.AreaId == cc.AreaId && cc.Id == dd.CarpetaEncabezadoid
                //                      select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }
                //                      ).ToList();

                //    // Agregado 06/05/2020:
                //    List<DocDetPermisos> documentos_Detalle;

                //    if (areasxrole.Count == 0)
                //    {
                //       var ListaDocumento = (from dd in db.Documentos_Detalle join a in db.Areas on dd.AreaId equals a.Id
                //                              join cc in db.ConfCarpetaEncabezados on dd.CarpetaEncabezadoid equals cc.Id
                //                              where cc.AreaId == a.Id && dd.RoleName == rname
                //                              select new { dd.Id, a.Descripcion, carpeta = cc.Descripcion, dd.llaveUnica, dd.Nombre_Ori, dd.Nombre_Des, dd.FechaRegistro, dd.FechaActualizacion }).ToList();

                //        documentos_Detalle = (from s in ListaDocumento
                //                              select new DocDetPermisos
                //                              {
                //                                  Id = s.Id,
                //                                  area = s.Descripcion,
                //                                  carpeta = s.carpeta,
                //                                  llaveunica = s.llaveUnica,
                //                                  Nombreori = s.Nombre_Ori,
                //                                  Nombredes = s.Nombre_Des,
                //                                  FechaRegistro = s.FechaRegistro,
                //                                  FechaActualizacion = s.FechaActualizacion

                //                              }).ToList();
                //    }
                //    else {
                //        documentos_Detalle = (from s in areasxrole
                //                              select new DocDetPermisos
                //                              {
                //                                  Id = s.Id,
                //                                  area = s.Descripcion,
                //                                  carpeta = s.carpeta,
                //                                  llaveunica = s.llaveUnica,
                //                                  Nombreori = s.Nombre_Ori,
                //                                  Nombredes = s.Nombre_Des,
                //                                  FechaRegistro = s.FechaRegistro,
                //                                  FechaActualizacion = s.FechaActualizacion

                //                              }).ToList();
                //    }

                //    return View(documentos_Detalle.ToList());

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
                //                              FechaRegistro = s.FechaRegistro,
                //                              FechaActualizacion = s.FechaActualizacion

                //                          }).ToList();
                //    //documentos_Detalle = documentos_Detalle.Where(s => s.llaveunica.Contains(Nombre));
                //    //documentos_Detalle = documentos_Detalle.Where(s => s.llaveunica.Contains(Nombre));
                //    return View(documentos_Detalle.ToList());
                //}

                //Modificado l 28/09/2020
                List<DocDetPermisos> documentos_Detalle = new List<DocDetPermisos>();
                return View(documentos_Detalle.ToList());
            }
            //return View(busqueda.ToList());

        }
        public ActionResult Imagen(int id)
        {
            //int id = 2;
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

        // ********************************* Agregado 07/05/2020
        // GET: Documento_Detalle/Details/5
        public async System.Threading.Tasks.Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);
            if (documento_Detalle == null)
            {
                return HttpNotFound();
            }
            Session["llaveUnica"] = documento_Detalle.llaveUnica;
            return View(documento_Detalle);
        }

        // GET: Documento_Detalle/Edit/5
        public async System.Threading.Tasks.Task<ActionResult> Edit(int? id, string llaveUnica)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);

            if (documento_Detalle == null)
            {
                return HttpNotFound();
            }

            List<SelectListItem> list = new List<SelectListItem>();
            //Modificado Septiembre 2020:
            Session["llaveUnica"] = llaveUnica;
            var documento_detalle = (from t in db.Documentos_Detalle
                                     where t.llaveUnica == llaveUnica
                                     select new { t.AreaId, t.CarpetaEncabezadoid }).ToList();
            int NAreaId = documento_detalle[0].AreaId;
            int NCarpetaEncabezadoid= documento_detalle[0].CarpetaEncabezadoid;
            //******************************
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
                ViewBag.NAreaId = NAreaId;
                ViewBag.NCarpetaEncabezadoid = NCarpetaEncabezadoid;
            }
            else
            {
                ViewBag.AreaId = list;
                //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion");
                ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion");
                ViewBag.NAreaId = NAreaId;
                ViewBag.NCarpetaEncabezadoid = NCarpetaEncabezadoid;
            }


            return View();

            //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", documento_Detalle.AreaId);
            //ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", documento_Detalle.CarpetaEncabezadoid);
            //return View(documento_Detalle);
        }

        //Agregado el 23/09/2020:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> Edit([Bind(Include = "Id,llaveUnica,AreaId,CarpetaEncabezadoid,Nombre_Ori,Nombre_Des,Imagen,FechaRegistro,FechaActualizacion,RoleName")] Documento_Detalle documento_Detalle, HttpPostedFileBase file, int NCarpetaEncabezadoid, int NAreaId)
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
            //documento_Detalle.AreaId = NAreaId;
            //documento_Detalle.CarpetaEncabezadoid = NCarpetaEncabezadoid;
            Session["llaveUnica"] = documento_Detalle.llaveUnica;
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
                    Console.Write(EX.Message);
                }

                nombrenuevo = documento_Detalle.llaveUnica + extension;

                //Modificado el 29/09/2020
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

                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                {

                    file.InputStream.CopyTo(ms);
                    array = ms.GetBuffer();
                    documento_Detalle.Imagen = array;
                }

                var context = new Models.ApplicationDbContext();
                documento_Detalle.Nombre_Ori = nombre;
                documento_Detalle.Nombre_Des = nombrenuevo;
                //Agregado:14042020
                var dateQuery = db.Database.SqlQuery<string>("select TO_CHAR(now(), 'DD/MM/YYYY HH24:MI:SS')");
                DateTime serverDate = Convert.ToDateTime(dateQuery.AsEnumerable().First());
                //DateTime FechaA = DateTime.Today;
                DateTime FechaA = serverDate;
                DateTime FechaR = context.Documentos_Detalle.FirstOrDefault(i => i.Id == documento_Detalle.Id).FechaRegistro;
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

                //Agregado:
                string norigen = "";
                //string ndestino = "";
                norigen= context.Documentos_Detalle.FirstOrDefault(i => i.Id == documento_Detalle.Id)?.Nombre_Ori;
                //ndestino = context.Documentos_Detalle.FirstOrDefault(i => i.Id == documento_Detalle.Id)?.Nombre_Des;
                documento_Detalle.Nombre_Ori = norigen;
                //Agregado:14042020
                nombre = documento_Detalle.Nombre_Ori.ToUpper();
                int posicionpunto = nombre.IndexOf(".");
                extension = nombre.Substring(posicionpunto);
                nombrenuevo = documento_Detalle.llaveUnica + extension;
                documento_Detalle.Nombre_Des = nombrenuevo;
                var dateQuery = db.Database.SqlQuery<string>("select TO_CHAR(now(), 'DD/MM/YYYY HH24:MI:SS')");
                DateTime serverDate = Convert.ToDateTime(dateQuery.AsEnumerable().First());
                //DateTime FechaA = DateTime.Today;
                DateTime FechaA = serverDate;
                DateTime FechaR = context.Documentos_Detalle.FirstOrDefault(i => i.Id == documento_Detalle.Id).FechaRegistro; 
                documento_Detalle.FechaActualizacion = FechaA;
                documento_Detalle.FechaRegistro = FechaR;
                DropboxListSubniveles rolenames = new DropboxListSubniveles();
                string rname = rolenames.rolename(User.Identity.Name.ToString());
                documento_Detalle.RoleName = rname;
                //Agregado:06052020

                //***************************************
                //Request.Flash("warning", "¡No se ha cargado ninguna imagén!");
                //return RedirectToAction("Edit");
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

                        db.Entry(documento_Detalle).State = System.Data.Entity.EntityState.Modified;
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
                Session["llaveUnica"] = documento_Detalle.llaveUnica;
                Request.Flash("success", "Documento actualizado correctamente");
                return RedirectToAction("Index");
            }

            //Modificado el 29/09/2020
            //db.Entry(documento_Detalle).State = System.Data.Entity.EntityState.Modified;
            //await db.SaveChangesAsync();
            ////*****************************************
            //ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", documento_Detalle.AreaId);
            //ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", documento_Detalle.CarpetaEncabezadoid);
            //Request.Flash("success", "Documento actualizado correctamente");
            ////return View(documento_Detalle);
            Session["llaveUnica"] = documento_Detalle.llaveUnica;
            return RedirectToAction("Index");
        }
        //*************************************************

        // GET: Documento_Detalle/Delete/5
        public async System.Threading.Tasks.Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);
            if (documento_Detalle == null)
            {
                return HttpNotFound();
            }
            Session["llaveUnica"] = documento_Detalle.llaveUnica;
            return View(documento_Detalle);
        }

        // POST: Documento_Detalle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> DeleteConfirmed(int id)
        {
            Documento_Detalle documento_Detalle = await db.Documentos_Detalle.FindAsync(id);
            db.Documentos_Detalle.Remove(documento_Detalle);
            await db.SaveChangesAsync();
            Request.Flash("success", "Documento eliminado correctamente");
            return RedirectToAction("Index");
        }

    }



}