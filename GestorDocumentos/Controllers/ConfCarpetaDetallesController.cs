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
    public class ConfCarpetaDetallesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ConfCarpetaDetalles
        public async Task<ActionResult> Index()
        {
            var confCarpetaDetalle = db.ConfCarpetaDetalle.Include(c => c.Areas).Include(c => c.ConfCarpetaEncabezado).Include(c => c.Tipo_Campo);
            //Modificado Septiembre 2020:
            string valor = Session["confCarpetaDetalle"] as string;
            if (valor != null && valor != "")
            {
                int nvalor = Convert.ToInt32(valor);
                Session["confCarpetaDetalle"] = "";
                return View(await confCarpetaDetalle.Where(x => x.Id == nvalor).ToListAsync());
            }
            return View(await confCarpetaDetalle.ToListAsync());
        }

        // GET: ConfCarpetaDetalles/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfCarpetaDetalle confCarpetaDetalle = await db.ConfCarpetaDetalle.FindAsync(id);
            if (confCarpetaDetalle == null)
            {
                return HttpNotFound();
            }
            Session["confCarpetaDetalle"] = confCarpetaDetalle.Id.ToString();
            return View(confCarpetaDetalle);
        }

        // GET: ConfCarpetaDetalles/Create
        public ActionResult Create()
        {
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo.OrderBy(x => x.Descripcion), "Id", "Descripcion");
            return View();
        }

        // POST: ConfCarpetaDetalles/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,NivelId,Descripcion,Longitud,AreaId,CarpetaEncabezadoid,Tipo_campoId")] ConfCarpetaDetalle confCarpetaDetalle)
        {
            //Verifica que no existan campo de descripción vacio
            if (confCarpetaDetalle.Descripcion == null) {
                Request.Flash("danger", "¡El campo de descripción esta vacio!");
                ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.AreaId);
                ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.CarpetaEncabezadoid);
                ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.Tipo_campoId);
                return View(confCarpetaDetalle);
            }

            //*******************************************************************************************

            //Verifica que no exista otra descripción detalle con el mismo nombre
            var lst = (from cd in db.ConfCarpetaDetalle join ce in db.ConfCarpetaEncabezados
                       on cd.CarpetaEncabezadoid equals ce.Id join a in db.Areas on cd.AreaId 
                       equals a.Id 
                       where cd.Descripcion.ToLower().Trim() == confCarpetaDetalle.Descripcion.ToLower().Trim()
                       && a.Id == confCarpetaDetalle.AreaId && ce.Id == confCarpetaDetalle.CarpetaEncabezadoid
                       select cd).ToList();
            if (lst.Count >= 1)
            {
                Request.Flash("warning", "¡La carpeta detalle ya existe, intente con otro nombre!");
                ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.AreaId);
                ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.CarpetaEncabezadoid);
                ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.Tipo_campoId);
                return View(confCarpetaDetalle);
            }

            //*******************************************************************************************

            //Verifica que no exista otro subnivel existente al máximo del encabezado (Ya no es necesario pero funciona)
            //var nivelmax = (from ced in db.ConfCarpetaDetalle
            //                where ced.AreaId == confCarpetaDetalle.AreaId
            //                && ced.CarpetaEncabezadoid == confCarpetaDetalle.CarpetaEncabezadoid
            //                select ced).Max(x => x.NivelId);

            //int subniveles = nivelmax;
            //int ce_nivel = confCarpetaDetalle.NivelId;

            //if (nivelmax == ce_nivel) {
            //    Request.Flash("warning", "¡La carpeta detalle ya tiene el máximo subnivel establecido!");
            //    ViewBag.AreaId = new SelectList(db.Areas, "Id", "Descripcion", confCarpetaDetalle.AreaId);
            //    ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados, "Id", "Descripcion", confCarpetaDetalle.CarpetaEncabezadoid);
            //    ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo, "Id", "Descripcion", confCarpetaDetalle.Tipo_campoId);
            //    return View(confCarpetaDetalle);
            //}
            //*******************************************************************************************

            if (ModelState.IsValid)
            {
                db.ConfCarpetaDetalle.Add(confCarpetaDetalle);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            //else
            //{
            //    if (confCarpetaDetalle.NivelId != 0 )
            //    {

            //        db.ConfCarpetaDetalle.Add(confCarpetaDetalle);
            //        await db.SaveChangesAsync();
            //        return RedirectToAction("Index");
            //    }
            //}

            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.CarpetaEncabezadoid);
            ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.Tipo_campoId);
            return View(confCarpetaDetalle);
        }

        // GET: ConfCarpetaDetalles/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfCarpetaDetalle confCarpetaDetalle = await db.ConfCarpetaDetalle.FindAsync(id);
            if (confCarpetaDetalle == null)
            {
                return HttpNotFound();
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.CarpetaEncabezadoid);
            ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.Tipo_campoId);
            Session["confCarpetaDetalle"] = confCarpetaDetalle.Id.ToString();
            return View(confCarpetaDetalle);
        }

        // POST: ConfCarpetaDetalles/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,NivelId,Descripcion,Longitud,AreaId,CarpetaEncabezadoid,Tipo_campoId")] ConfCarpetaDetalle confCarpetaDetalle)
        {
            //Modificado el 12/05/2020 para capturar el areaid y carpetaencabezadoid
            List<SelectListItem> lcodigos = (from cd in db.ConfCarpetaDetalle
                                             where cd.Id == confCarpetaDetalle.Id
                                             select new SelectListItem { Value = cd.AreaId.ToString(), Text = cd.CarpetaEncabezadoid.ToString() }).ToList();

            confCarpetaDetalle.AreaId = Convert.ToInt32(lcodigos[0].Value);
            confCarpetaDetalle.CarpetaEncabezadoid = Convert.ToInt32(lcodigos[0].Text);

            if (ModelState.IsValid)
            {
                db.Entry(confCarpetaDetalle).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AreaId = new SelectList(db.Areas.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.AreaId);
            ViewBag.CarpetaEncabezadoid = new SelectList(db.ConfCarpetaEncabezados.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.CarpetaEncabezadoid);
            ViewBag.Tipo_campoId = new SelectList(db.Tipo_Campo.OrderBy(x => x.Descripcion), "Id", "Descripcion", confCarpetaDetalle.Tipo_campoId);
            return View(confCarpetaDetalle);
        }

        // GET: ConfCarpetaDetalles/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ConfCarpetaDetalle confCarpetaDetalle = await db.ConfCarpetaDetalle.FindAsync(id);
            if (confCarpetaDetalle == null)
            {
                return HttpNotFound();
            }
            Session["confCarpetaDetalle"] = confCarpetaDetalle.Id.ToString();
            return View(confCarpetaDetalle);
        }

        // POST: ConfCarpetaDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ConfCarpetaDetalle confCarpetaDetalle = await db.ConfCarpetaDetalle.FindAsync(id);
            db.ConfCarpetaDetalle.Remove(confCarpetaDetalle);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public JsonResult TraerTipoCampoDescripcion(int id)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaniveles = subniveles.GetTipoCampo(id);
            return Json(listaniveles, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAreayEncabezadoCarpetaId(int idarea)
        {
            DropboxListSubniveles subniveles = new DropboxListSubniveles();
            var listaareas = subniveles.GetAreayEncabezadoCarpeta(idarea);
            return Json(listaareas, JsonRequestBehavior.AllowGet);
        }

        //Controlar los niveles 2/1/2020
        public JsonResult ControlarNivelesMax(int id1, int id2)
        {
            DropboxListSubniveles controlmax = new DropboxListSubniveles();
            var maximo = controlmax.GetControlarNivelesMax(id1, id2);
            return Json(maximo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerificaSubnivelMaximo(int id1, int id2) {
            List<SelectListItem> list = new List<SelectListItem>();

            //Agregado: 27/04/2020
            int nivelmaximo = 0;

            List<SelectListItem> lista = (from t in db.ConfCarpetaDetalle
                                          where t.AreaId == id1 && t.CarpetaEncabezadoid == id2
                                          orderby t.NivelId descending
                                          select new SelectListItem()
                                          {
                                              Value = t.NivelId.ToString()
                                          }).Take(1).ToList();
            int valor = lista.Count;

            if (valor == 0)
            {
                nivelmaximo = 1;
            }
            else {
                var nivelmax = (from ced in db.ConfCarpetaDetalle
                                where ced.AreaId == id1
                                && ced.CarpetaEncabezadoid == id2
                                select ced).Max(x => x.NivelId);
                nivelmaximo = nivelmax;
            }
            //*************************************************



            //var nivelmax = (from ced in db.ConfCarpetaDetalle
            //                                where ced.AreaId == id1
            //                                && ced.CarpetaEncabezadoid == id2
            //                             select ced).Max(x => x.NivelId);

            var ce_nivelmax=(from ce in db.ConfCarpetaEncabezados where ce.AreaId == id1
                             && ce.Id == id2
                             select ce).Max(x => x.Subniveles);

            if (nivelmaximo >= ce_nivelmax) {
                list.Add(new SelectListItem() { Value = "1", Text = "1" });
            }
            else
            {
                list.Add(new SelectListItem() { Value = "2", Text = "2" });
            }
       
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMostrarNivelesEnEditar(int id1, int id2, string descripcion) {
            List<SelectListItem> Lista = (from cd in db.ConfCarpetaDetalle
                                          where cd.AreaId == id1 && cd.CarpetaEncabezadoid == id2
                                          && cd.Descripcion == descripcion
                                          select new SelectListItem
                                          {
                                              Value = cd.Id.ToString(),
                                              Text=cd.NivelId.ToString()
                                          }).ToList();
            return Json(Lista, JsonRequestBehavior.AllowGet);
        }
        ///############################################

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
