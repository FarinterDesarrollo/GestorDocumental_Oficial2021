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

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class AreasController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Areas
        public async Task<ActionResult> Index()
        {
            //Modificado Septiembre 2020:
            string valor = Session["Areas"] as string;
            if (valor != null && valor != "")
            {
                int nvalor = Convert.ToInt32(valor);
                Session["Areas"] = "";
                return View(await db.Areas.Where(x => x.Id == nvalor).ToListAsync());
            }
            return View(await db.Areas.ToListAsync());
        }

        // GET: Areas/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Areas areas = await db.Areas.FindAsync(id);
            if (areas == null)
            {
                return HttpNotFound();
            }
            Session["Areas"] = areas.Id.ToString();
            return View(areas);
        }

        // GET: Areas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Areas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion")] Areas areas)
        {
            var ar = (from a in db.Areas where a.Descripcion.ToLower().Trim() == areas.Descripcion.ToLower().Trim() select a.Descripcion).ToList();

            if (ar.Count >=1)
            {
                Request.Flash("warning", "¡El area ya existe, intente con otro nombre!");
                return View(areas);
            }

            if (ModelState.IsValid)
            {
                db.Areas.Add(areas);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(areas);
        }

        // GET: Areas/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Areas areas = await db.Areas.FindAsync(id);
            if (areas == null)
            {
                return HttpNotFound();
            }
            Session["Areas"] = areas.Id.ToString();
            return View(areas);
        }

        // POST: Areas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion")] Areas areas)
        {
            var ar = (from a in db.Areas where a.Descripcion.ToLower().Trim() == areas.Descripcion.ToLower().Trim() select a.Descripcion).ToList();

            if (ar.Count >= 1)
            {
                Request.Flash("warning", "¡El area ya existe, intente con otro nombre!");
                return View(areas);
            }

            if (ModelState.IsValid)
            {
                db.Entry(areas).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(areas);
        }

        // GET: Areas/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Areas areas = await db.Areas.FindAsync(id);
            if (areas == null)
            {
                return HttpNotFound();
            }
            Session["Areas"] = areas.Id.ToString();
            return View(areas);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Areas areas = await db.Areas.FindAsync(id);

            var lst = (from a in db.Areas
                       join ce in db.ConfCarpetaEncabezados on a.Id equals ce.AreaId
                       where a.Id == id
                       select new { a.Id, ce.Descripcion }).ToList();

            if (lst.Count >= 1)
            {
                Request.Flash("danger", "¡No se puede eliminar el área puesto que hay registros asociados!");
                return View(areas);
            }

            db.Areas.Remove(areas);
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
