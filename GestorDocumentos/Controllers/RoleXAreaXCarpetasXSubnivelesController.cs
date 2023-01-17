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
using Microsoft.AspNet.Identity.Owin;
using GestorDocumentos.Archivos;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class RoleXAreaXCarpetasXSubnivelesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public RoleXAreaXCarpetasXSubnivelesController()
        {
        }

        public RoleXAreaXCarpetasXSubnivelesController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        // GET: RoleXAreaXCarpetasXSubniveles
        public async Task<ActionResult> Index()
        {
            var roleXAreaXCarpetasXSubniveles = db.RoleXAreaXCarpetasXSubniveles.Include(r => r.Areas).Include(r => r.ConfCarpetaDetalle).Include(r => r.ConfCarpetaEncabezado).Include(r => r.MantenimientoSubniveles);
            //Modificado Septiembre 2020:
            string valor = Session["RolxAreaxCarpetaxSubNivelesId"] as string;
            if (valor != null && valor != "")
            {
                int nvalor = Convert.ToInt32(valor);
                roleXAreaXCarpetasXSubniveles = (from t in roleXAreaXCarpetasXSubniveles where t.id == nvalor select t);
                Session["RolxAreaxCarpetaxSubNivelesId"] = "";
            }
            return View(await roleXAreaXCarpetasXSubniveles.ToListAsync());
        }

        // GET: RoleXAreaXCarpetasXSubniveles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXAreaXCarpetasXSubniveles roleXAreaXCarpetasXSubniveles = await db.RoleXAreaXCarpetasXSubniveles.FindAsync(id);
            if (roleXAreaXCarpetasXSubniveles == null)
            {
                return HttpNotFound();
            }
            Session["RolxAreaxCarpetaxSubNivelesId"] = roleXAreaXCarpetasXSubniveles.id.ToString();
            return View(roleXAreaXCarpetasXSubniveles);
        }

        // GET: RoleXAreaXCarpetasXSubniveles/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            ViewBag.ConfCarpetaDetalleid = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            ViewBag.MantenimientoSubnivelesid = new SelectList(db.MantenimientoSubniveles.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            return View();
        }

        // POST: RoleXAreaXCarpetasXSubniveles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,RoleName,AreaId,CarpetaEncabezadoid,ConfCarpetaDetalleid,MantenimientoSubnivelesid")] RoleXAreaXCarpetasXSubniveles roleXAreaXCarpetasXSubniveles)
        {
            if (ModelState.IsValid)
            {
                db.RoleXAreaXCarpetasXSubniveles.Add(roleXAreaXCarpetasXSubniveles);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.AreaId);
            ViewBag.ConfCarpetaDetalleid = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.ConfCarpetaDetalleid);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.CarpetaEncabezadoid);
            ViewBag.MantenimientoSubnivelesid = new SelectList(db.MantenimientoSubniveles.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.MantenimientoSubnivelesid);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            return View(roleXAreaXCarpetasXSubniveles);
        }

        // GET: RoleXAreaXCarpetasXSubniveles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles)
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            RoleXAreaXCarpetasXSubniveles roleXAreaXCarpetasXSubniveles = await db.RoleXAreaXCarpetasXSubniveles.FindAsync(id);
            if (roleXAreaXCarpetasXSubniveles == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.AreaId);
            ViewBag.ConfCarpetaDetalleid = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.ConfCarpetaDetalleid);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.CarpetaEncabezadoid);
            ViewBag.MantenimientoSubnivelesid = new SelectList(db.MantenimientoSubniveles.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.MantenimientoSubnivelesid);
            ViewBag.Role = list;
            Session["RolxAreaxCarpetaxSubNivelesId"] = roleXAreaXCarpetasXSubniveles.id.ToString();
            return View(roleXAreaXCarpetasXSubniveles);
        }

        // POST: RoleXAreaXCarpetasXSubniveles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,RoleName,AreaId,CarpetaEncabezadoid,ConfCarpetaDetalleid,MantenimientoSubnivelesid")] RoleXAreaXCarpetasXSubniveles roleXAreaXCarpetasXSubniveles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleXAreaXCarpetasXSubniveles).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.AreaId);
            ViewBag.ConfCarpetaDetalleid = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.ConfCarpetaDetalleid);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.CarpetaEncabezadoid);

            ViewBag.MantenimientoSubnivelesid = new SelectList(db.MantenimientoSubniveles.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpetasXSubniveles.MantenimientoSubnivelesid);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            return View(roleXAreaXCarpetasXSubniveles);
        }

        // GET: RoleXAreaXCarpetasXSubniveles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXAreaXCarpetasXSubniveles roleXAreaXCarpetasXSubniveles = await db.RoleXAreaXCarpetasXSubniveles.FindAsync(id);
            if (roleXAreaXCarpetasXSubniveles == null)
            {
                return HttpNotFound();
            }
            Session["RolxAreaxCarpetaxSubNivelesId"] = roleXAreaXCarpetasXSubniveles.id.ToString();
            return View(roleXAreaXCarpetasXSubniveles);
        }

        // POST: RoleXAreaXCarpetasXSubniveles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RoleXAreaXCarpetasXSubniveles roleXAreaXCarpetasXSubniveles = await db.RoleXAreaXCarpetasXSubniveles.FindAsync(id);
            db.RoleXAreaXCarpetasXSubniveles.Remove(roleXAreaXCarpetasXSubniveles);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public JsonResult GetAreayEncabezadoCarpetaId(int idarea)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaareas = subniveles.GetAreayEncabezadoCarpeta(idarea);
            return Json(listaareas, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCarpetaDetalleEncabezadoId(int idcarpeta)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaniveles = subniveles.GetCarpetaDetalleEncabezadoId(idcarpeta);
            return Json(listaniveles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubnivelesId(int idcarpetadetalle)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaniveles = subniveles.GetSubnivelesId(idcarpetadetalle);
            return Json(listaniveles, JsonRequestBehavior.AllowGet);
        }
    }
}
