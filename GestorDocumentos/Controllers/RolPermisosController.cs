using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GestorDocumentos.Models;

namespace GestorDocumentos.Controllers
{
    public class RolPermisosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RolPermisos
        public ActionResult Index()
        {
            var rolPermisos = db.RolPermisos.Include(r => r.Pantallas);
            string valor = Session["BUsuarioId"] as string;
            if (valor != null && valor != "")
            {
                int nid = Convert.ToInt32(valor);
                rolPermisos = (from t in rolPermisos where t.id == nid select t);
                Session["BUsuarioId"] = "";
            }
            return View(rolPermisos.ToList());
        }

        // GET: RolPermisos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolPermisos rolPermisos = db.RolPermisos.Find(id);
            if (rolPermisos == null)
            {
                return HttpNotFound();
            }

            //Modificado Septiembre 2020:
            Session["BUsuarioId"] = rolPermisos.id.ToString();
            return View(rolPermisos);
        }

        // GET: RolPermisos/Create
        public ActionResult Create()
        {
            ViewBag.IdPantalla = new SelectList(db.Pantallas, "IdPantalla", "pantalla");
            return View();
        }

        // POST: RolPermisos/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,RoleName,IdPantalla,Pantalla,consultar,crear,editar,eliminar")] RolPermisos rolPermisos)
        {
            if (rolPermisos.RoleName == null)
            {
                ViewBag.IdPantalla = new SelectList(db.Pantallas, "IdPantalla", "pantalla", rolPermisos.IdPantalla);
                return View(rolPermisos);
            }

            List<RoleViewModels> rol = (from r in db.Roles
                                        where r.Id == rolPermisos.RoleName
                                        select new RoleViewModels { Name = r.Name}).ToList();

            rolPermisos.RoleName = rol[0].Name;

            //List<Pantallas> npantalla = (from p in db.Pantallas
            //                             where p.IdPantalla == rolPermisos.IdPantalla
            //                             select new Pantallas { pantalla = p.pantalla }).ToList();

            //rolPermisos.Pantalla = npantalla[0].pantalla;

            if (ModelState.IsValid)
            {
                db.RolPermisos.Add(rolPermisos);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IdPantalla = new SelectList(db.Pantallas, "IdPantalla", "pantalla", rolPermisos.IdPantalla);
            return View(rolPermisos);
        }

        // GET: RolPermisos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolPermisos rolPermisos = db.RolPermisos.Find(id);
            if (rolPermisos == null)
            {
                return HttpNotFound();
            }
            ViewBag.IdPantalla = new SelectList(db.Pantallas, "IdPantalla", "pantalla", rolPermisos.IdPantalla);
            //Modificado Septiembre 2020:
            Session["BUsuarioId"] = rolPermisos.id.ToString();
            return View(rolPermisos);
        }

        // POST: RolPermisos/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,RoleName,IdPantalla,Pantalla,consultar,crear,editar,eliminar")] RolPermisos rolPermisos)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rolPermisos).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IdPantalla = new SelectList(db.Pantallas, "IdPantalla", "pantalla", rolPermisos.IdPantalla);
            return View(rolPermisos);
        }

        // GET: RolPermisos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RolPermisos rolPermisos = db.RolPermisos.Find(id);
            if (rolPermisos == null)
            {
                return HttpNotFound();
            }
            Session["BUsuarioId"] = rolPermisos.id.ToString();
            return View(rolPermisos);
        }

        // POST: RolPermisos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RolPermisos rolPermisos = db.RolPermisos.Find(id);
            db.RolPermisos.Remove(rolPermisos);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetRoles()
        {
            List<SelectListItem> lista = (from r in db.Roles.OrderBy(x => x.Name) select new SelectListItem { Value = r.Id, Text = r.Name }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCriterios()
        {
            List<SelectListItem> valores = new List<SelectListItem>();

            valores.Add(new SelectListItem() { Text = "SI", Value = "1" });
            valores.Add(new SelectListItem() { Text = "NO", Value = "2" });

            SelectList lista = new SelectList(valores, "Value", "Text");
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPantalla(int id) {
            List<SelectListItem> lista = (from p in db.Pantallas where p.IdPantalla == id select new SelectListItem { Value = p.IdPantalla.ToString(), Text = p.pantalla }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        // Para llenar la parte de editar

        public JsonResult GetPermisosConsulta(string pantalla, string rname)
        {

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.consultar
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermisosCrear(string pantalla, string rname)
        {
           
            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.crear
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermisosEditar(string pantalla, string rname)
        {

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.editar
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPermisosEliminar(string pantalla, string rname)
        {

            List<SelectListItem> rp = (from x in db.RolPermisos
                                       where x.RoleName == rname && x.Pantalla == pantalla
                                       select new SelectListItem
                                       {
                                           Value = x.id.ToString(),
                                           Text = x.eliminar
                                       }).ToList();
            return Json(rp, JsonRequestBehavior.AllowGet);
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
