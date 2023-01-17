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
    //[Authorize]
    public class Tipo_CampoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tipo_Campo
        public async Task<ActionResult> Index()
        {
            return View(await db.Tipo_Campo.ToListAsync());
        }

        // GET: Tipo_Campo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Campo tipo_Campo = await db.Tipo_Campo.FindAsync(id);
            if (tipo_Campo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Campo);
        }

        // GET: Tipo_Campo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tipo_Campo/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion,Longitud,Automatico")] Tipo_Campo tipo_Campo)
        {
            if (ModelState.IsValid)
            {
                db.Tipo_Campo.Add(tipo_Campo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(tipo_Campo);
        }

        // GET: Tipo_Campo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Campo tipo_Campo = await db.Tipo_Campo.FindAsync(id);
            if (tipo_Campo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Campo);
        }

        // POST: Tipo_Campo/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,Longitud,Automatico")] Tipo_Campo tipo_Campo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipo_Campo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(tipo_Campo);
        }

        // GET: Tipo_Campo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tipo_Campo tipo_Campo = await db.Tipo_Campo.FindAsync(id);
            if (tipo_Campo == null)
            {
                return HttpNotFound();
            }
            return View(tipo_Campo);
        }

        // POST: Tipo_Campo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Tipo_Campo tipo_Campo = await db.Tipo_Campo.FindAsync(id);
            db.Tipo_Campo.Remove(tipo_Campo);
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
