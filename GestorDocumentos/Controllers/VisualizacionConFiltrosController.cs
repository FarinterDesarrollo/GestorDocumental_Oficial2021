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
using GestorDocumentos.Servicios;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class VisualizacionConFiltrosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: VisualizacionConFiltros
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        private readonly FarmaciasServices _services = new FarmaciasServices();
        public VisualizacionConFiltrosController()
        {
        }

        public VisualizacionConFiltrosController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
                         where axr.RoleName == rname
                         select new { a.Id, a.Descripcion }).ToList();

            //Agregado el 31 de Diciembre de 2020
            string encontro = Session["docEncontro"] as string;
            if (encontro == "N")
            {
                Session["docEncontro"] = "";
                ViewBag.Message = "No se encontraron registros...";
            }
            else
            {
                Session["docEncontro"] = "";
            }
            //***********************************

            if (areas.Count == 0)
            {
                var areasxcarpeta = (from a in db.Areas
                                     join axrxc in db.RoleXAreaXCarpetas on
                                     a.Id equals axrxc.AreaId
                                     where axrxc.RoleName == rname
                                     select new { a.Id, a.Descripcion }).Distinct().ToList();
                if (areasxcarpeta.Count == 0)
                {
                    var areasxcarpetaxubniveles = (from a in db.Areas
                                                   join axrxcxs in db.RoleXAreaXCarpetasXSubniveles on
                                                   a.Id equals axrxcxs.AreaId
                                                   where axrxcxs.RoleName == rname
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

            if(rname.Contains("JEFE DE FARMACIA"))
            {
                ViewBag.Rol = rname.Substring(0, 16);
            }
            else
            {
                ViewBag.Rol = rname;
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

            var Efarmacia = (from ms in db.MantenimientoSubniveles join rxaxcxs in db.RoleXAreaXCarpetasXSubniveles
                             on ms.CarpetaEncabezadoid equals rxaxcxs.CarpetaEncabezadoid
                             where ms.Id == rxaxcxs.MantenimientoSubnivelesid && rxaxcxs.RoleName == rname
                             && ms.CarpetaEncabezadoid == id
                             select ms.Id).ToList();

            var mn = db.MantenimientoSubniveles.OrderBy(x => x.Descripcion).Where(ms => ms.CarpetaEncabezadoid == id);

            if (Efarmacia.Count > 0)
            {
                int numero = Efarmacia[0];
                //var subnivel = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && (ms.Descripcion.Contains("K0") || ms.Descripcion.Contains("K1") || ms.Descripcion.Contains("K2") || ms.Descripcion.Contains("K5") || ms.Descripcion.Contains("K6"))).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && (ms.Descripcion.Contains("H0") || ms.Descripcion.Contains("H1") || ms.Descripcion.Contains("H2") || ms.Descripcion.Contains("H5") || ms.Descripcion.Contains("H6")))).ToList();
                var subnivel = _services.MS_Farmacias_PermisosLista(id, numero, 1);
                
                if (subnivel.Count > 0)
                {
                    //mn = db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && !ms.Descripcion.Contains("K0") && !ms.Descripcion.Contains("K1") && !ms.Descripcion.Contains("K2") && !ms.Descripcion.Contains("K5") && !ms.Descripcion.Contains("K6") && !ms.Descripcion.Contains("H0") && !ms.Descripcion.Contains("H1") && !ms.Descripcion.Contains("H2") && !ms.Descripcion.Contains("H5") && !ms.Descripcion.Contains("H6")).Union(db.MantenimientoSubniveles.Where(ms => ms.CarpetaEncabezadoid == id && ms.Id == numero)); //original 20211112
                    mn = _services.MS_Farmacias_Permisos(id, numero, 2);
                }
                else
                {
                    // Nota: Es para roles que no son de farmacias.

                    mn = (from ms in db.MantenimientoSubniveles
                          join rxaxcxs in db.RoleXAreaXCarpetasXSubniveles
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
            var listaareas = subniveles.GetAreayEncabezadoCarpetaIdpermisos(idarea, rname);
            return Json(listaareas, JsonRequestBehavior.AllowGet);
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
