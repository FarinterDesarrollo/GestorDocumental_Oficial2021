using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using GestorDocumentos.Archivos;
using GestorDocumentos.Models;

namespace GestorDocumentos.Controllers
{
    public class PantallasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Pantallas
        public ActionResult Index()
        {
            return View(db.Pantallas.ToList());
        }

        // GET: Pantallas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pantallas pantallas = db.Pantallas.Find(id);
            if (pantallas == null)
            {
                return HttpNotFound();
            }
            return View(pantallas);
        }

        // GET: Pantallas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Pantallas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPantalla,pantalla")] Pantallas pantallas)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var listapantallas = (from p in db.Pantallas
                              where p.pantalla.Trim() == pantallas.pantalla.Trim()
                              select new { p.pantalla }).ToList();

            if (listapantallas.Count >= 1)
            {
                Request.Flash("danger", "¡El nombre de pantalla ya existe, intente con otro nombre!");
                return View();
            }

            if (ModelState.IsValid)
            {
                db.Pantallas.Add(pantallas);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(pantallas);
        }

        // GET: Pantallas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pantallas pantallas = db.Pantallas.Find(id);
            if (pantallas == null)
            {
                return HttpNotFound();
            }
            return View(pantallas);
        }

        // POST: Pantallas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdPantalla,pantalla")] Pantallas pantallas)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var listapantallas = (from p in db.Pantallas
                                  where p.pantalla.Trim() == pantallas.pantalla.Trim()
                                  select new { p.pantalla }).ToList();

            if (listapantallas.Count >= 1)
            {
                Request.Flash("danger", "¡El nombre de pantalla ya existe, intente con otro nombre!");
                return View();
            }

            if (ModelState.IsValid)
            {
                db.Entry(pantallas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(pantallas);
        }

        // GET: Pantallas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Pantallas pantallas = db.Pantallas.Find(id);
            if (pantallas == null)
            {
                return HttpNotFound();
            }
            return View(pantallas);
        }

        // POST: Pantallas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Pantallas pantallas = db.Pantallas.Find(id);
            db.Pantallas.Remove(pantallas);
            db.SaveChanges();
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
