using GestorDocumentos.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExcelDataReader;
using System.Data;
using GestorDocumentos.Clases;
using System.Net;
using System.Globalization;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private ApplicationRoleManager _roleManager;
        public ApplicationDbContext db = new ApplicationDbContext();

        public RoleController()
        {
        }

        public RoleController(ApplicationRoleManager roleManager)
        {
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

        public ActionResult Index() {
            List<RoleViewModels> list = new List<RoleViewModels>();
            foreach (var role in RoleManager.Roles) {
                list.Add(new RoleViewModels(role));
            }
            //Modificado Septiembre 2020:
            string RolId = Session["RolId"] as string;
            if (RolId != null && RolId != "")
            {
                list = (from t in list where t.Id == RolId select t).ToList();
                Session["RolId"] = "";
            }
            return View(list);
        }

        public ActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModels model) {
            var role = new ApplicationRole() { Name = model.Name };

            // Validación 13/03/2020:
            if(role.Name is null)
            {
                Request.Flash("warning", "¡Tiene que llenar el cuadro de texto nombre de rol!");
                return View();
            }

            // Validación 11/03/2020:
            ApplicationDbContext db = new ApplicationDbContext();

            var listaroles = (from r in db.Roles
                              where r.Name.Trim() == model.Name.Trim()
                              select new { r.Name }).ToList();

            if (listaroles.Count >= 1)
            {
                Request.Flash("danger", "¡El rol ya existe, intente con otro nombre!");
                return View();
            }

            await RoleManager.CreateAsync(role);
            return RedirectToAction("Index");
        }

        // *** creado en la fecha: 09/06/2022 carga masiva roles
        public ActionResult Carga_Masiva_Roles()
        {
            string VSPCCS_NoExcel = Session["VSPCCS_NoExcel"] as string;
            if (VSPCCS_NoExcel == "Y")
            {
                ViewBag.VSPCCS_NoExcel = VSPCCS_NoExcel;
                Session["VSPCCS_NoExcel"] = "";
            }

            string Excel_SinDatos = Session["Excel_SinDatos"] as string;
            if (Excel_SinDatos == "Y")
            {
                ViewBag.Excel_SinDatos = Excel_SinDatos;
                Session["Excel_SinDatos"] = "";
            }

            string Excel_Columnas = Session["Excel_Columnas"] as string;
            if (Excel_Columnas == "Y")
            {
                ViewBag.Excel_Columnas = Excel_Columnas;
                Session["Excel_Columnas"] = "";
            }

            string Excel_NombreColumna = Session["Excel_NombreColumna"] as string;
            if (Excel_NombreColumna == "Y")
            {
                ViewBag.Excel_NombreColumna = Excel_NombreColumna;
                Session["Excel_NombreColumna"] = "";
            }

            string Excel_Rol = Session["Excel_Rol"] as string;
            if (Excel_Rol == "Y")
            {
                ViewBag.Excel_Rol = Excel_Rol;
                Session["Excel_Rol"] = "";
            }

            string Excel_RolCarEsp = Session["Excel_RolCarEsp"] as string;
            if (Excel_RolCarEsp == "Y")
            {
                ViewBag.Excel_RolCarEsp = Excel_RolCarEsp;
                Session["Excel_RolCarEsp"] = "";
            }

            string Excel_RolRepetidos = Session["Excel_RolRepetidos"] as string;
            if (Excel_RolRepetidos == "Y")
            {
                ViewBag.Excel_RolRepetidos = Excel_RolRepetidos;
                Session["Excel_RolRepetidos"] = "";
            }

            string Excel_Cargado = Session["Excel_Cargado"] as string;
            if (Excel_Cargado == "Y")
            {
                ViewBag.Excel_Cargado = Excel_Cargado;
                Session["Excel_Cargado"] = "";
            }

            string Excel_Error = Session["Excel_Error"] as string;
            if (Excel_Error == "Y")
            {
                ViewBag.Excel_Error = "Error";
                Session["Excel_Error"] = "";
            }

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Carga_Masiva_Roles(RoleViewModels model, HttpPostedFileBase file)
        {
            try
            {
                // --- Creación del archivo de excel
                // Validar Archivo
                if (file != null && file.ContentLength > 0)
                {
                    string AspNetRoles = "-AspNetRoles-";
                    AspNetRoles = AspNetRoles.Replace('-', '"');
                    string Name = "-Name-";
                    Name = Name.Replace('-', '"');

                    // Valida si es un archivo de excel
                    if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx"))
                    {
                        Session["VSPCCS_NoExcel"] = "Y";
                        // Mensaje
                        goto y;
                    }
                    // .xlsx
                    IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(file.InputStream);
                    //// reader.IsFirstRowAsColumnNames
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    DataSet dataSet = reader.AsDataSet(conf);
                    DataTable tabla = new DataTable();
                    tabla = dataSet.Tables[0];

                    //Validación si el archivo de excel contiene datos
                    if (tabla.Rows.Count == 0)
                    {
                        Session["Excel_SinDatos"] = "Y"; //Variable global sin datos
                        // Mensaje
                        goto y;
                    }

                    //Validación si el archivo de excel no contiene el número de columnas requerido
                    if ((tabla.Columns.Count < 1) || (tabla.Columns.Count > 1))
                    {
                        Session["Excel_Columnas"] = "Y"; //Variable global columnas requeridas "Y" si no cumple
                        // Mensaje
                        goto y;
                    }

                    //Validación si el archivo de excel no contiene el nombre de columna requerido
                    if ((tabla.Columns[0].ColumnName != "Rol"))
                    {
                        Session["Excel_NombreColumna"] = "Y"; //Variable global nombre columna requeridas "Y" si no cumple
                        // Mensaje
                        goto y;
                    }

                    //Validación si el archivo de excel contiene datos repetidos, columna rol
                    var ColumnaRol = new List<string>();
                    foreach (DataRow k in tabla.Rows)
                    {
                        ColumnaRol.Add(k[0].ToString());
                    }

                    // Columna rol
                    var x = from ob in ColumnaRol group ob by ob into g select new { Name = g.Key, Duplicatecount = g.Count() };
                    foreach (var m in x)
                    {
                        Console.WriteLine(m.Name + "--" + m.Duplicatecount);
                        if ((m.Name == null) || (m.Name == ""))
                        {
                            Session["Excel_Rol"] = "Y"; //Variable global rol "Y" si no contiene valores
                            // Mensaje
                            goto y;
                        }

                        // Verifica si el rol no contiene caracteres especiales
                        var withoutSpecial = new string(m.Name.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c) || Char.IsPunctuation(c)).ToArray());
                        string vname = m.Name;

                        //Si tiene caracteres especiales
                        if (vname != withoutSpecial)
                        {
                            Session["Excel_RolCarEsp"] = "Y"; //Variable global caracteres especiales "Y" si contiene 
                            goto y;
                        }

                        // Nombres repetidos
                        if (m.Duplicatecount > 1)
                        {
                            Session["Excel_RolRepetidos"] = "Y"; //Variable global repetidos "Y" si contiene 
                            // Mensaje
                            goto y;
                        }
                    }

                    // *********** Inicio Proceso Final de validación e inserción de datos ***********
                    List<Cls_DatosExcel_Detalle> DatosExcel = new List<Cls_DatosExcel_Detalle>();
                    string rol = "";

                    foreach (DataRow dr in dataSet.Tables[0].Rows) // Validación si esta registrado el rol
                    {
                        //Muestras los valores obteniendolos con el Índice o el Nombre de la columna, 
                        //de la siguiente manera:

                        rol = dr["Rol"].ToString();
                        rol = rol.Trim();

                        var MQuery = db.Database.SqlQuery<Cls_DatosExcel_Detalle>("SELECT " + Name + " FROM dbo." + AspNetRoles + " WHERE " + Name + "='" + rol + "'").ToList();

                        if (MQuery.Count == 0)
                        {
                            DatosExcel.Add(new Cls_DatosExcel_Detalle
                            {
                                Name = rol,
                                Status = "Exitoso"
                            });
                        }
                        else
                        {
                            DatosExcel.Add(new Cls_DatosExcel_Detalle
                            {
                                Name = rol,
                                Status = "Ya existe en tabla AspNetRoles"
                            });
                        }
                    }

                    foreach (var item in DatosExcel)
                    {
                        if (item.Status == "Exitoso")
                        {
                            //Guardar información en la tabla usuarios
                            model.Name = item.Name;

                            var role = new ApplicationRole() { Name = model.Name };

                            await RoleManager.CreateAsync(role);

                            Session["Excel_Cargado"] = "Y";
                        }
                    }

                    var myExport = new CsvExport();
                    foreach (var items in DatosExcel)
                    {
                        myExport.AddRow();
                        myExport["Rol"] = items.Name;
                        myExport["Status"] = items.Status;
                    }

                    byte[] myCsvData = myExport.ExportToBytes();

                    //// Obtiene la respuesta actual
                    System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                    // Tipo de contenido para forzar la descarga
                    response.ContentType = "application/octet-stream";
                    response.AddHeader("Content-Disposition", "attachment; filename=" + "Estado de carga roles " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.InvariantCulture) + ".csv");
                    response.StatusCode = (int)HttpStatusCode.OK;
                    // Envia los bytes
                    response.BinaryWrite(myCsvData);
                    HttpContext.Response.Flush(); // '// Sends all currently buffered output To the client.
                    HttpContext.Response.SuppressContent = true; // '// Gets Or sets a value indicating whether To send HTTP content To the client.
                    HttpContext.ApplicationInstance.CompleteRequest();

                    // Borra la respuesta
                    response.Clear();
                    response.ClearContent();
                    // *********** FIN Proceso Final de validación e inserción de datos ***********
                }
                //Fin archivo de excel
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Session["Excel_Error"] = "Y";
            }
        y:
            //return View(model);
            return Redirect("Carga_Masiva_Roles");
        }

        private class Cls_DatosExcel_Detalle
        {
            public string Name { get; set; }
            public string Status { get; set; }
        }

        [HttpPost]
        public ActionResult Descargar_Formato()
        {
            try
            {
                var myExport = new CsvExport();
                string Email = ""; 
                myExport.AddRow();
                myExport["Rol"] = Email;

                byte[] myCsvData = myExport.ExportToBytes();

                //// Obtiene la respuesta actual
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                // Tipo de contenido para forzar la descarga
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-Disposition", "attachment; filename=" + "Formato de carga roles " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.InvariantCulture) + ".xls");
                response.StatusCode = (int)HttpStatusCode.OK;
                // Envia los bytes
                response.BinaryWrite(myCsvData);
                HttpContext.Response.Flush(); // '// Sends all currently buffered output To the client.
                HttpContext.Response.SuppressContent = true; // '// Gets Or sets a value indicating whether To send HTTP content To the client.
                HttpContext.ApplicationInstance.CompleteRequest();

                // Borra la respuesta
                response.Clear();
                response.ClearContent();

                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                    list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
                ViewBag.Roles = list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Redirect("Carga_Masiva_Roles");
        }
        // *** Fin carga masiva roles

        public async Task<ActionResult> Edit(string id) {
            var role = await RoleManager.FindByIdAsync(id);
            //Modificado Septiembre 2020:
            Session["RolId"] = id;
            return View(new RoleViewModels(role));
        }
        [HttpPost]
        public async Task<ActionResult> Edit(RoleViewModels model)
        {
            var role = new ApplicationRole() { Id = model.Id, Name = model.Name };
            // Validación 11/03/2020:
            ApplicationDbContext db = new ApplicationDbContext();

            var listaroles = (from r in db.Roles where r.Name.Trim() == model.Name.Trim()
                              select new { r.Name }).ToList();

            if (listaroles.Count >= 1)
            {
                return View();
            }

            //**************************************************************

            await RoleManager.UpdateAsync(role);
            return View(new RoleViewModels(role));
        }

        // Validación 11/03/2020:

        public JsonResult AvisoRolExistente(string nombre)
      {
            ApplicationDbContext db = new ApplicationDbContext();

            var listaroles = (from r in db.Roles
                              where r.Name.Trim() == nombre.Trim()
                              select new { r.Name}).ToList();

            return Json(listaroles, JsonRequestBehavior.AllowGet);
        }

        //**************************************************************


        public async Task<ActionResult> Details(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            //Modificado Septiembre 2020:
            Session["RolId"] = id;
            return View(new RoleViewModels(role));
        }

        public async Task<ActionResult> Delete(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);
            Session["RolId"] = id;
            return View(new RoleViewModels(role));

        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            ApplicationDbContext db = new ApplicationDbContext();
            var lst = (from raxc in db.RoleXAreaXCarpetas
                       where raxc.RoleName == role.Name
                       select raxc).ToList();
            if (lst.Count>=1)
            {
                Request.Flash("danger", "¡No se puede eliminar el rol puesto que hay registros asociados!");
                return View(new RoleViewModels(role));
            }

            await RoleManager.DeleteAsync(role);
            return RedirectToAction("Index");
        }
    }
}
