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
    public class ConfCarpetaEncabezadoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ConfCarpetaEncabezadoes
        public async Task<ActionResult> Index()
        {
            var confCarpetaEncabezados = db.ConfCarpetaEncabezados.Include(c => c.Areas);
            //Modificado Septiembre 2020:
            string valor = Session["confCarpetaEncabezados"] as string;
            if (valor != null && valor != "")
            {
                int nvalor = Convert.ToInt32(valor);
                Session["confCarpetaEncabezados"] = "";
                return View(await confCarpetaEncabezados.Where(x => x.Id == nvalor).ToListAsync());
            }
            return View(await confCarpetaEncabezados.ToListAsync());
        }

        // GET: ConfCarpetaEncabezadoes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfCarpetaEncabezado confCarpetaEncabezado = await db.ConfCarpetaEncabezados.FindAsync(id);
            if (confCarpetaEncabezado == null)
            {
                return HttpNotFound();
            }
            Session["confCarpetaEncabezados"] = confCarpetaEncabezado.Id.ToString();
            return View(confCarpetaEncabezado);
        }

        // GET: ConfCarpetaEncabezadoes/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            return View();
        }

        // POST: ConfCarpetaEncabezadoes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion,Subniveles,AreaId")] ConfCarpetaEncabezado confCarpetaEncabezado)
        {
            var lst = (from ce in db.ConfCarpetaEncabezados join a in db.Areas on ce.AreaId equals a.Id
                       where ce.Descripcion.ToLower().Trim() == confCarpetaEncabezado.Descripcion.ToLower().Trim()
                       && a.Id == confCarpetaEncabezado.AreaId
                       select  ce).ToList();
            if (lst.Count >= 1)
            {
                Request.Flash("warning", "¡La carpeta ya existe, intente con otro nombre!");
                ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaEncabezado.AreaId);
                return View(confCarpetaEncabezado);
            }

            if (ModelState.IsValid)
            {
                db.ConfCarpetaEncabezados.Add(confCarpetaEncabezado);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaEncabezado.AreaId);
            return View(confCarpetaEncabezado);
        }

        // GET: ConfCarpetaEncabezadoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfCarpetaEncabezado confCarpetaEncabezado = await db.ConfCarpetaEncabezados.FindAsync(id);
            if (confCarpetaEncabezado == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaEncabezado.AreaId);
            Session["confCarpetaEncabezados"] = confCarpetaEncabezado.Id.ToString();
            return View(confCarpetaEncabezado);
        }

        // POST: ConfCarpetaEncabezadoes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,Subniveles,AreaId")] ConfCarpetaEncabezado confCarpetaEncabezado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(confCarpetaEncabezado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaEncabezado.AreaId);
            return View(confCarpetaEncabezado);
        }

        // GET: ConfCarpetaEncabezadoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfCarpetaEncabezado confCarpetaEncabezado = await db.ConfCarpetaEncabezados.FindAsync(id);
            if (confCarpetaEncabezado == null)
            {
                return HttpNotFound();
            }
            Session["confCarpetaEncabezados"] = confCarpetaEncabezado.Id.ToString();
            return View(confCarpetaEncabezado);
        }

        // POST: ConfCarpetaEncabezadoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ConfCarpetaEncabezado confCarpetaEncabezado = await db.ConfCarpetaEncabezados.FindAsync(id);
            db.ConfCarpetaEncabezados.Remove(confCarpetaEncabezado);
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
