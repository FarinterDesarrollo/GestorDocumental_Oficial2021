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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class RoleXAreasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public RoleXAreasController()
        {
        }

        public RoleXAreasController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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
        // GET: RoleXAreas
        public async Task<ActionResult> Index()
        {
            var roleXAreas = db.RoleXAreas.Include(r => r.Areas);
            //Modificado Septiembre 2020:
            string valor = Session["RoleXAreasId"] as string;
            if (valor != null && valor != "")
            {
                int nvalor = Convert.ToInt32(valor);
                Session["RoleXAreasId"] = "";
                roleXAreas = (from t in roleXAreas where t.id == nvalor select t);
            }
            return View(await roleXAreas.ToListAsync());
        }

        // GET: RoleXAreas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXArea roleXArea = await db.RoleXAreas.FindAsync(id);
            if (roleXArea == null)
            {
                return HttpNotFound();
            }
            Session["RoleXAreasId"] = roleXArea.id.ToString();
            return View(roleXArea);
        }

        // GET: RoleXAreas/Create
        public ActionResult Create()
        {

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");

            return View();
        }

        // POST: RoleXAreas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,RoleName,AreaId")] RoleXArea roleXArea)
        {
            if (ModelState.IsValid)
            {
                db.RoleXAreas.Add(roleXArea);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXArea.AreaId);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            return View(roleXArea);
        }

        // GET: RoleXAreas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXArea roleXArea = await db.RoleXAreas.FindAsync(id);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");

            if (roleXArea == null)
            {
                return HttpNotFound();
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", roleXArea.AreaId);
            ViewBag.Roles = list;
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            Session["RoleXAreasId"] = roleXArea.id.ToString();
            return View(roleXArea);
        }

        // POST: RoleXAreas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,RoleName,AreaId")] RoleXArea roleXArea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(roleXArea).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", roleXArea.AreaId);
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            return View(roleXArea);
        }

        // GET: RoleXAreas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RoleXArea roleXArea = await db.RoleXAreas.FindAsync(id);
            if (roleXArea == null)
            {
                return HttpNotFound();
            }
            Session["RoleXAreasId"] = roleXArea.id.ToString();
            return View(roleXArea);
        }

        // POST: RoleXAreas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            RoleXArea roleXArea = await db.RoleXAreas.FindAsync(id);
            db.RoleXAreas.Remove(roleXArea);
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
    }
}
