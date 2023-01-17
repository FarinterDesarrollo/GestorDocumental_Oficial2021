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

namespace GestorDocumentos.Controllers
{
    public class RoleXAreaXCarpetasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public RoleXAreaXCarpetasController()
        {
        }

        public RoleXAreaXCarpetasController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        // GET: RoleXAreaXCarpetas
        public async Task<ActionResult> Index()
        {
            var roleXAreaXCarpetas = db.RoleXAreaXCarpetas.Include(r => r.Areas).Include(r => r.ConfCarpetaEncabezado);
            //Modificado Septiembre 2020:
            string valor = Session["RolxAreaxCarpetaId"] as string;
            if (valor != null && valor != "")
            {
                int nvalor = Convert.ToInt32(valor);
                roleXAreaXCarpetas = (from t in roleXAreaXCarpetas where t.id == nvalor select t);
                Session["RolxAreaxCarpetaId"] = "";
            }

            return View(await roleXAreaXCarpetas.ToListAsync());
        }

        // GET: RoleXAreaXCarpetas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXAreaXCarpeta roleXAreaXCarpeta = await db.RoleXAreaXCarpetas.FindAsync(id);
            if (roleXAreaXCarpeta == null)
            {
                return HttpNotFound();
            }
            Session["RolxAreaxCarpetaId"] = roleXAreaXCarpeta.id.ToString();
            return View(roleXAreaXCarpeta);
        }

        // GET: RoleXAreaXCarpetas/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion").Distinct();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            
            return View();
        }

        // POST: RoleXAreaXCarpetas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,RoleName,AreaId,CarpetaEncabezadoid")] RoleXAreaXCarpeta roleXAreaXCarpeta)
        {
            if (ModelState.IsValid)
            {
                db.RoleXAreaXCarpetas.Add(roleXAreaXCarpeta);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpeta.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpeta.CarpetaEncabezadoid);
           
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            return View(roleXAreaXCarpeta);
        }

        // GET: RoleXAreaXCarpetas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            RoleXAreaXCarpeta roleXAreaXCarpeta = await db.RoleXAreaXCarpetas.FindAsync(id);
            if (roleXAreaXCarpeta == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpeta.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpeta.CarpetaEncabezadoid);
            ViewBag.Roles = list;
            Session["RolxAreaxCarpetaId"] = roleXAreaXCarpeta.id.ToString();
            return View(roleXAreaXCarpeta);
        }

        // POST: RoleXAreaXCarpetas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,RoleName,AreaId,CarpetaEncabezadoid")] RoleXAreaXCarpeta roleXAreaXCarpeta)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleXAreaXCarpeta).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpeta.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXAreaXCarpeta.CarpetaEncabezadoid);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            return View(roleXAreaXCarpeta);
        }

        // GET: RoleXAreaXCarpetas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXAreaXCarpeta roleXAreaXCarpeta = await db.RoleXAreaXCarpetas.FindAsync(id);
            if (roleXAreaXCarpeta == null)
            {
                return HttpNotFound();
            }
            Session["RolxAreaxCarpetaId"] = roleXAreaXCarpeta.id.ToString();
            return View(roleXAreaXCarpeta);
        }

        // POST: RoleXAreaXCarpetas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RoleXAreaXCarpeta roleXAreaXCarpeta = await db.RoleXAreaXCarpetas.FindAsync(id);
            db.RoleXAreaXCarpetas.Remove(roleXAreaXCarpeta);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public JsonResult GetCarpetaEncabezado(int idarea) {
            List<SelectListItem> cc = (from ce in db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion)
                                       where ce.AreaId == idarea
                                       select new SelectListItem
                                       {
                                           Value = ce.Id.ToString(),
                                           Text = ce.Descripcion
                                       }).ToList();

            return Json(cc, JsonRequestBehavior.AllowGet);
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
