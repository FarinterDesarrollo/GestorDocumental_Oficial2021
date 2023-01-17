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
using GestorDocumentos.Archivos;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class MantenimientoSubnivelesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: MantenimientoSubniveles
        public ActionResult Index(string Nombre)
        {
            //Modificado Septiembre 2020:
            string valor = Session["mantenimientoSubniveles"] as string;
            if (valor != null && valor != "")
            {
                Nombre = valor;
                Session["mantenimientoSubniveles"] = "";
            }

            if (Nombre == null) {
                var mantenimientoSubniveles = (from a in db.Areas
                                               join ce in db.ConfCarpetaEncabezados on a.Id equals ce.AreaId
                                               join cd in db.ConfCarpetaDetalle on ce.Id equals cd.CarpetaEncabezadoid
                                               join ms in db.MantenimientoSubniveles on a.Id equals ms.AreaId
                                               where cd.AreaId == ce.AreaId && cd.AreaId == a.Id && ms.AreaId == ce.AreaId
                                               && ms.AreaId == cd.AreaId && ms.CarpetaEncabezadoid == ce.Id && ms.ConfCarpetaDetalleId == cd.Id
                                               select new { a.Descripcion, cdDescripcion = cd.Descripcion, ceDescripcion = ce.Descripcion, msDescripcion = ms.Descripcion,ms.Id }).ToList();

                List<ListadeSubniveles> lista_mantenimiento = (from m in mantenimientoSubniveles
                                                               select new ListadeSubniveles
                                                               {
                                                                   Area=m.Descripcion,
                                                                   Detalle=m.cdDescripcion,
                                                                   Encabezado=m.ceDescripcion,
                                                                   Mantenimiento=m.msDescripcion,
                                                                   Id = m.Id
                                                               }).ToList();

                return View(lista_mantenimiento);
            }
            else
            {
                //Modificado Septiembre 2020:
                List<ListadeSubniveles> lista_mantenimiento = new List<ListadeSubniveles>();
                if (valor != null && valor != "")
                {
                    int nvalor = Convert.ToInt32(valor);
                    var mantenimientoSubniveles = (from a in db.Areas
                                                   join ce in db.ConfCarpetaEncabezados on a.Id equals ce.AreaId
                                                   join cd in db.ConfCarpetaDetalle on ce.Id equals cd.CarpetaEncabezadoid
                                                   join ms in db.MantenimientoSubniveles on a.Id equals ms.AreaId
                                                   where cd.AreaId == ce.AreaId && cd.AreaId == a.Id && ms.AreaId == ce.AreaId
                                                   && ms.AreaId == cd.AreaId && ms.CarpetaEncabezadoid == ce.Id && ms.ConfCarpetaDetalleId == cd.Id
                                                   && ms.Id == nvalor
                                                   select new { a.Descripcion, cdDescripcion = cd.Descripcion, ceDescripcion = ce.Descripcion, msDescripcion = ms.Descripcion, ms.Id }).ToList();

                    lista_mantenimiento = (from m in mantenimientoSubniveles
                                           select new ListadeSubniveles
                                           {
                                               Area = m.Descripcion,
                                               Detalle = m.cdDescripcion,
                                               Encabezado = m.ceDescripcion,
                                               Mantenimiento = m.msDescripcion,
                                               Id = m.Id
                                           }).ToList();
                }
                else
                {
                   var mantenimientoSubniveles = (from a in db.Areas
                                                   join ce in db.ConfCarpetaEncabezados on a.Id equals ce.AreaId
                                                   join cd in db.ConfCarpetaDetalle on ce.Id equals cd.CarpetaEncabezadoid
                                                   join ms in db.MantenimientoSubniveles on a.Id equals ms.AreaId
                                                   where cd.AreaId == ce.AreaId && cd.AreaId == a.Id && ms.AreaId == ce.AreaId
                                                   && ms.AreaId == cd.AreaId && ms.CarpetaEncabezadoid == ce.Id && ms.ConfCarpetaDetalleId == cd.Id
                                                   && (ce.Descripcion.Contains(Nombre) || ms.Descripcion.Contains(Nombre))
                                                   select new { a.Descripcion, cdDescripcion = cd.Descripcion, ceDescripcion = ce.Descripcion, msDescripcion = ms.Descripcion, ms.Id }).ToList();

                    lista_mantenimiento = (from m in mantenimientoSubniveles
                                                                   select new ListadeSubniveles
                                                                   {
                                                                       Area = m.Descripcion,
                                                                       Detalle = m.cdDescripcion,
                                                                       Encabezado = m.ceDescripcion,
                                                                       Mantenimiento = m.msDescripcion,
                                                                       Id = m.Id
                                                                   }).ToList();
                }

                return View(lista_mantenimiento);
            }
            //var mantenimientoSubniveles = db.MantenimientoSubniveles.Include(m => m.Areas).Include(m => m.ConfCarpetaDetalle).Include(m => m.ConfCarpetaEncabezado);
            //return View(await mantenimientoSubniveles.ToListAsync());
        }

        // GET: MantenimientoSubniveles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MantenimientoSubniveles mantenimientoSubniveles = await db.MantenimientoSubniveles.FindAsync(id);
            if (mantenimientoSubniveles == null)
            {
                return HttpNotFound();
            }
            Session["mantenimientoSubniveles"] = mantenimientoSubniveles.Id.ToString();
            return View(mantenimientoSubniveles);
        }

        public JsonResult GetCarpetaDetalleEncabezadoId(int idcarpeta)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaniveles = subniveles.GetCarpetaDetalleEncabezadoId(idcarpeta);
            return Json(listaniveles, JsonRequestBehavior.AllowGet);
        }

        // GET: MantenimientoSubniveles/Create
        public ActionResult Create()
        {
            //ViewBag.ConfCarpetaDetalleId = new SelectList(db.ConfCarpetaDetalle, "Id", "Descripcion");
            //ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion");
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaniveles = subniveles.GetCarpetaEncabezadoId();
            ViewBag.ListaSubniveles = listaniveles;
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            return View();
        }

        // POST: MantenimientoSubniveles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AreaId,CarpetaEncabezadoid,ConfCarpetaDetalleId,Descripcion")] MantenimientoSubniveles mantenimientoSubniveles)
        {
            if (ModelState.IsValid)
            {
                db.MantenimientoSubniveles.Add(mantenimientoSubniveles);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.AreaId);
            ViewBag.ConfCarpetaDetalleId = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.ConfCarpetaDetalleId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.CarpetaEncabezadoid);
            return View(mantenimientoSubniveles);
        }

        // GET: MantenimientoSubniveles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MantenimientoSubniveles mantenimientoSubniveles = await db.MantenimientoSubniveles.FindAsync(id);
            if (mantenimientoSubniveles == null)
            {
                return HttpNotFound();
            }

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.AreaId);
            ViewBag.ConfCarpetaDetalleId = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.ConfCarpetaDetalleId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.CarpetaEncabezadoid);
            Session["mantenimientoSubniveles"] = mantenimientoSubniveles.Id.ToString();
            return View(mantenimientoSubniveles);
        }

        // POST: MantenimientoSubniveles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AreaId,CarpetaEncabezadoid,ConfCarpetaDetalleId,Descripcion")] MantenimientoSubniveles mantenimientoSubniveles)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mantenimientoSubniveles).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.AreaId);
            ViewBag.ConfCarpetaDetalleId = new SelectList(db.ConfCarpetaDetalle.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.ConfCarpetaDetalleId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", mantenimientoSubniveles.CarpetaEncabezadoid);
            return View(mantenimientoSubniveles);
        }

        // GET: MantenimientoSubniveles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MantenimientoSubniveles mantenimientoSubniveles = await db.MantenimientoSubniveles.FindAsync(id);
            if (mantenimientoSubniveles == null)
            {
                return HttpNotFound();
            }
            Session["mantenimientoSubniveles"] = mantenimientoSubniveles.Id.ToString();
            return View(mantenimientoSubniveles);
        }

        // POST: MantenimientoSubniveles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MantenimientoSubniveles mantenimientoSubniveles = await db.MantenimientoSubniveles.FindAsync(id);
            db.MantenimientoSubniveles.Remove(mantenimientoSubniveles);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        //Cambio realizado el 03/01/2020
        public JsonResult GetAreayEncabezadoCarpetaId(int idarea)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaareas = subniveles.GetAreayEncabezadoCarpeta(idarea);
            return Json(listaareas, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetControlarTipodeCampoId(int id1, int id2, int id3)
        {
            DropboxListSubniveles Lista = new DropboxListSubniveles();
            var listaTipo = Lista.GetControlarTipodeCampo(id1, id2, id3);
            return Json(listaTipo, JsonRequestBehavior.AllowGet);
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
