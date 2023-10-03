using GestorDocumentos.Clases;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GestorDocumentos.Servicios;
using GestorDocumentos.Models.Request;
using GestorDocumentos.Models.Response;
using GestorDocumentos.Models;
using OfficeOpenXml;
using Newtonsoft.Json.Linq;

namespace GestorDocumentos.Controllers
{
    public class FarmaciasSubNivelesVisualizarController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly Conexion conexion = new Conexion();
        private readonly Globales globales = new Globales();
        private readonly Sqlpg sqlpg = new Sqlpg();
        private readonly Sql sql = new Sql();
        private readonly FarmaciasServices farmaciasService = new FarmaciasServices();
        // GET: FarmaciasSubNivelesVisualizar
        public ActionResult Index()
        {
            List<FarmaciasEncabezadoResponse> listModel = new List<FarmaciasEncabezadoResponse>();
            List<DabaBaseLdcom> bd = new List<DabaBaseLdcom>();
            bd = farmaciasService.BasedeDatosConec();
            ViewBag.ListaFarmacias = new SelectList(bd.OrderBy(x => x.id), "id", "name");

            listModel = farmaciasService.GetFarmaciasEncabezadoResponses(0, 0);

            return View(listModel);
        }

        // GET: FarmaciasSubNivelesVisualizar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: FarmaciasSubNivelesVisualizar/Create
        public ActionResult Create(string mensaje, string mensajeTipo)
        {
            List<DabaBaseLdcom> bd = new List<DabaBaseLdcom>();
            bd= farmaciasService.BasedeDatosConec();
            ViewBag.ListaFarmacias = new SelectList(bd.OrderBy(x=>x.id), "id", "name");
            FarmaciasEncabezadoRequest model = new FarmaciasEncabezadoRequest();

            if(mensajeTipo == "exito")
            {
                ViewBag.Success = mensaje;
            }
            else if (mensajeTipo == "error")
            {
                ViewBag.Error = mensaje;
            }
            else
            {
                ViewBag.Aviso = mensaje;
            }

            return View(model);
        }

        // POST: FarmaciasSubNivelesVisualizar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "baseDatos,empId,empNombre,cadenaNombre,aliasCadena,abreviacion")] FarmaciasEncabezadoRequest values)
        {
            try
            {
                // TODO: Add insert logic here
                string mensaje = ""; string mensajeTipo = "";
                string guardar = farmaciasService.GuardarInformacionFarmacias(values);
                if(guardar == "OK")
                {
                    mensajeTipo = "exito";
                    mensaje = "Farmacias registradas satisfactoriamente.";
                }
                else if(guardar == "EMP EXISTS")
                {
                    mensajeTipo = "error";
                    mensaje = $"La empresa {values.empNombre} ya existe.";
                }
                else if(guardar == "FE NOT FOUND")
                {
                    mensajeTipo = "error";
                    mensaje = "No se puedo registrar el encabezado, vuelva intentar o comuníquese con el desarrollador.";
                }
                else
                {
                    mensajeTipo = "error";
                    mensaje = "Se genero un error interno, informar al desarrollador.";
                }

                return RedirectToAction("Create", "FarmaciasSubNivelesVisualizar", new {mensaje= mensaje, mensajeTipo= mensajeTipo });
            }
            catch
            {
                return View();
            }
        }

        public JsonResult GeMostrarData(string bdName)
        {
            try
            {
                List<EmpXCadenaResponse> empXcadenaResponse = new List<EmpXCadenaResponse>();
                List<ErrorResponse> errorResponse = new List<ErrorResponse>();

                if (bdName != null && bdName !="")
                {
                    var empresa = farmaciasService.EmpresaLsConec(bdName);
                    if(empresa[0].error != null)
                    {
                        errorResponse.Add(new ErrorResponse
                        {
                            error = empresa[0].error
                        });

                        return Json(errorResponse, JsonRequestBehavior.AllowGet);
                    }

                    var cadena = farmaciasService.CadenaLsConec(bdName);
                    if (cadena[0].error != null)
                    {
                        errorResponse.Add(new ErrorResponse
                        {
                            error = cadena[0].error
                        });

                        return Json(errorResponse, JsonRequestBehavior.AllowGet);
                    }

                    foreach (var item in empresa)
                    {
                        foreach (var item2 in cadena)
                        {
                            empXcadenaResponse.Add(new EmpXCadenaResponse
                            {
                                empId = item.emp_id,
                                empNombre = item.emp_nombre,
                                cadenaNombre = item2.cadena_nombre
                            });
                        }
                    }

                    return Json(empXcadenaResponse, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errorResponse.Add(new ErrorResponse
                    {
                        error = "Objeto vacio."
                    });

                    return Json(errorResponse, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                List<ErrorResponse> errorResponse = new List<ErrorResponse>();
                Console.WriteLine(ex.Message);
                errorResponse.Add(new ErrorResponse
                {
                    error = ex.Message
                });
                return Json(errorResponse, JsonRequestBehavior.AllowGet); 
            }
        }

        // GET: FarmaciasSubNivelesVisualizar/Edit/5
        public ActionResult Edit(short empId, string mensaje, string mensajeTipo)
        {
            FarmaciasSubNivelesRequest model = new FarmaciasSubNivelesRequest();
            FarmaciasSubnivelEncabezado me = new FarmaciasSubnivelEncabezado();
            List<FarmaciasSubnivelDetalle> md = new List<FarmaciasSubnivelDetalle>();

            if (mensajeTipo == "exito")
            {
                ViewBag.Success = mensaje;
            }
            else if (mensajeTipo == "error")
            {
                ViewBag.Error = mensaje;
            }
            else
            {
                ViewBag.Aviso = mensaje;
            }

            if (empId == 0)
            {
                model = new FarmaciasSubNivelesRequest
                {
                    farmaciasEncabezado = me,
                    farmaciasDetalle = md
                };

                return View(model);
            }

            globales.query = $"select * from dbo.farmacias_subnivel_encabezado where emp_id={empId}";
            DataTable dtEncabezado = new DataTable();
            dtEncabezado = sqlpg.ddt(globales.query, conexion.Conectar());
            if(dtEncabezado.Rows.Count > 0)
            {
                foreach(DataRow item in dtEncabezado.Rows)
                {
                    me.id = item.Field<int>("id");
                    me.emp_id = item.Field<short>("emp_id");
                    me.emp_nombre = item.Field<string>("emp_nombre");
                    me.cadena_nombre = item.Field<string>("cadena_nombre");
                    me.alias_cadena = item.Field<string>("alias_cadena");
                    me.notacion = item.Field<string>("notacion");
                }
            }

            globales.query = $"select * from dbo.farmacias_subnivel_detalle where fe_id={me.id}";
            DataTable dtDetalle = new DataTable();
            dtDetalle = sqlpg.ddt(globales.query, conexion.Conectar());
            if(dtDetalle.Rows.Count > 0)
            {
                foreach(DataRow item in dtDetalle.Rows)
                {
                    md.Add(new FarmaciasSubnivelDetalle
                    {
                        consecutivo=item.Field<int>("consecutivo"),
                        fe_id=item.Field<int>("fe_id"),
                        suc_id=item.Field<short>("suc_id"),
                        suc_nombre=item.Field<string>("suc_nombre"),
                        abreviacion=item.Field<string>("abreviacion")
                    });
                }
            }

            model = new FarmaciasSubNivelesRequest
            {
                farmaciasEncabezado = me,
                farmaciasDetalle = md
            };

            return View(model);
        }

        // POST: FarmaciasSubNivelesVisualizar/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, short emp_id, FarmaciasSubnivelEncabezado model)
        {
            try
            {
                string mensaje = ""; string mensajeTipo = "";
                // TODO: Add update logic here
                if (model == null)
                {
                    mensajeTipo = "error";
                    mensaje = "No se obtuvo el valor seleccionado.";
                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = emp_id });
                }

                if(emp_id == 0)
                {
                    mensajeTipo = "error";
                    mensaje = "No se obtuvo el valor de empresa.";
                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = emp_id });
                }

                if (id == 0)
                {
                    mensajeTipo = "error";
                    mensaje = "No se obtuvo el valor del encabezado.";
                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = emp_id });
                }


                globales.query = $"select emp_nombre from dbo.farmacias_subnivel_encabezado where id not in({id}) and emp_id not in({emp_id}) and alias_cadena='{model.alias_cadena}'";
                string aliasExiste = sqlpg.get(globales.query, conexion.Conectar());
                if(aliasExiste.Length > 0)
                {
                    mensajeTipo = "error";
                    mensaje = $"El alias de cadena ya existe para la empresa: {aliasExiste}.";
                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = emp_id });
                }

                JObject obj = new JObject();
                if(model.alias_cadena != null && model.alias_cadena != "")
                {
                    obj.Add("alias_cadena", $"'{model.alias_cadena}'");
                }

                if (model.notacion != null && model.notacion != "")
                {
                    obj.Add("notacion", $"'{model.notacion}'");
                }

                string campos = "";
                string elementos = "";
                int i = 0;

                foreach (var reg in obj)
                {
                    i += 1;

                    if (i == 1)
                    {
                        campos = reg.Key + "=" + reg.Value + ",";
                    }
                    else
                    {
                        elementos = reg.Key + "=" + reg.Value + ",";
                        campos = campos + elementos;
                    }
                }

                int indice = campos.LastIndexOf(",");
                campos = campos.Substring(0, indice);

                globales.query = $@"update dbo.farmacias_subnivel_encabezado set {campos} where id={id} and emp_id={emp_id}";
                sqlpg.save(globales.query, conexion.Conectar());

                mensajeTipo = "exito";
                mensaje = "Encabezado modificado satisfactoriamente.";
                return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = emp_id });
            }
            catch(Exception ex)
            {
                string mensaje = ""; string mensajeTipo = "";
                mensajeTipo = "error";
                mensaje = ex.Message;
                return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = emp_id });
            }
        }

        public ActionResult GuardarAbreviacion(short empId, int consecutivo, string abreviacion, string sucursal)
        {
            try
            {
                string mensaje = ""; string mensajeTipo = "";

                if (consecutivo != 0)
                {
                    JObject obj = new JObject();
                    if (abreviacion != null && abreviacion != "")
                    {
                        abreviacion = abreviacion.Trim();
                        obj.Add("abreviacion", $"'{abreviacion}'");
                    }

                    if (sucursal != null && sucursal != "")
                    {
                        sucursal = sucursal.Trim();
                        obj.Add("suc_nombre", $"'{sucursal}'");
                    }

                    string campos = "";
                    string elementos = "";
                    int i = 0;

                    foreach (var reg in obj)
                    {
                        i += 1;

                        if (i == 1)
                        {
                            campos = reg.Key + "=" + reg.Value + ",";
                        }
                        else
                        {
                            elementos = reg.Key + "=" + reg.Value + ",";
                            campos = campos + elementos;
                        }
                    }

                    int indice = campos.LastIndexOf(",");
                    campos = campos.Substring(0, indice);


                    globales.query = $"UPDATE dbo.farmacias_subnivel_detalle SET {campos} WHERE consecutivo={consecutivo}";
                    sqlpg.save(globales.query, conexion.Conectar());

                    mensajeTipo = "exito";
                    mensaje = "Valor modificado satisfactoriamente.";

                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                }
                else
                {
                    mensajeTipo = "error";
                    mensaje = "No se obtuvo el valor seleccionado.";
                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                }
            }
            catch(Exception ex)
            {
                string mensaje = ""; string mensajeTipo = "";
                mensajeTipo = "error";
                mensaje = ex.Message;
                return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
            }
        }

        public ActionResult Recargar(string mensaje, string mensajeTipo)
        {
            try
            {
                FarmaciasEncabezadoRequest model = new FarmaciasEncabezadoRequest();
                List<DabaBaseLdcom> bd = new List<DabaBaseLdcom>();
                bd = farmaciasService.BasedeDatosConec();
                ViewBag.ListaFarmacias = new SelectList(bd.OrderBy(x => x.id), "id", "name");

                if (mensajeTipo == "exito")
                {
                    ViewBag.Success = mensaje;
                }
                else if (mensajeTipo == "error")
                {
                    ViewBag.Error = mensaje;
                }
                else
                {
                    ViewBag.Aviso = mensaje;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                FarmaciasSubnivelEncabezado model = new FarmaciasSubnivelEncabezado();
                ViewBag.Error = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Recargar(FarmaciasEncabezadoRequest values)
        {
            try
            {
                string mensaje = ""; string mensajeTipo = "";
                string recargar = farmaciasService.RecargarInformacionFarmacias(values);
                if (recargar == "OK")
                {
                    mensajeTipo = "exito";
                    mensaje = "Farmacias registradas satisfactoriamente.";
                }
                else if (recargar == "EMP NOT EXISTS")
                {
                    mensajeTipo = "aviso";
                    mensaje = $"La empresa aun no esta {values.empNombre} registrada.";
                }
                else if(recargar == "SIN CAMBIOS")
                {
                    mensajeTipo = "aviso";
                    mensaje = $"No detecto ningun cambio para {values.empNombre} a nivel de LDCOM.";
                }
                else
                {
                    mensajeTipo = "error";
                    mensaje = "Se genero un error interno, informar al desarrollador.";
                }

                return RedirectToAction("Recargar", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo });
            }
            catch(Exception ex)
            {
                string mensaje = ""; string mensajeTipo = "";
                mensajeTipo = "error";
                mensaje = ex.Message;
                return RedirectToAction("Recargar", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo });
            }
        }

        public JsonResult GetRecargar(string bdName)
        {
            try
            {
                List<EmpXCadenaResponse> empXcadenaResponse = new List<EmpXCadenaResponse>();
                List<ErrorResponse> errorResponse = new List<ErrorResponse>();

                if (bdName != null && bdName != "")
                {
                    var empresa = farmaciasService.EmpresaLsConec(bdName);
                    if (empresa[0].error != null)
                    {
                        errorResponse.Add(new ErrorResponse
                        {
                            error = empresa[0].error
                        });

                        return Json(errorResponse, JsonRequestBehavior.AllowGet);
                    }

                    foreach (var item in empresa)
                    {
                        globales.query = $"select notacion from dbo.farmacias_subnivel_encabezado where emp_id={item.emp_id}";
                        string notacion = sqlpg.get(globales.query, conexion.Conectar());

                        empXcadenaResponse.Add(new EmpXCadenaResponse
                        {
                            empId = item.emp_id,
                            empNombre = item.emp_nombre,
                            notacion = notacion
                        });
                    }

                    return Json(empXcadenaResponse, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errorResponse.Add(new ErrorResponse
                    {
                        error = "Objeto vacio."
                    });

                    return Json(errorResponse, JsonRequestBehavior.AllowGet);
                }
            }
            catch(Exception ex)
            {
                List<ErrorResponse> errorResponse = new List<ErrorResponse>();
                Console.WriteLine(ex.Message);
                errorResponse.Add(new ErrorResponse
                {
                    error = ex.Message
                });
                return Json(errorResponse, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: FarmaciasSubNivelesVisualizar/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: FarmaciasSubNivelesVisualizar/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase archivoInput, int emp_id2)
        {
            try
            {
                string mensaje = "";
                string mensajeTipo = "";
                int empId = 0;

                if(emp_id2 > 0)
                {
                    empId = emp_id2;
                }

                if (archivoInput != null && archivoInput.ContentLength > 0)
                {
                    ExcelPackage.LicenseContext = LicenseContext.Commercial;
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(archivoInput.InputStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0]; // Suponiendo que los datos están en la primera hoja
                        List<FarmaciasSubnivelDetalle> fd = new List<FarmaciasSubnivelDetalle>();
                        // Aquí puedes procesar los datos de la hoja de Excel (worksheet)
                        // Por ejemplo, leer celdas y guardar los datos en una lista
                        // Ejemplo:

                        // ******* Validación de existencia de encabezados del archivo *******
                        string tituloId = worksheet.Cells[1, 1].Text.Trim();
                        string tituloFe = worksheet.Cells[1, 2].Text.Trim();
                        string tituloSucId = worksheet.Cells[1, 3].Text.Trim();
                        string tituloSucNombre = worksheet.Cells[1, 4].Text.Trim();
                        string tituloAbreviacion = worksheet.Cells[1, 5].Text.Trim();

                        if(tituloId != "Id")
                        {
                            mensajeTipo = "aviso";
                            mensaje = "El archivo no posee el campo Id.";
                            return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                        }

                        if (tituloFe != "Fe Id")
                        {
                            mensajeTipo = "aviso";
                            mensaje = "El archivo no posee el campo fe id.";
                            return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                        }

                        if (tituloSucId != "Suc Id")
                        {
                            mensajeTipo = "aviso";
                            mensaje = "El archivo no posee el campo Suc Id.";
                            return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                        }

                        if (tituloSucNombre != "Suc Nombre")
                        {
                            mensajeTipo = "aviso";
                            mensaje = "El archivo no posee el campo Suc Nombre.";
                            return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                        }

                        if (tituloAbreviacion != "Abreviación")
                        {
                            mensajeTipo = "aviso";
                            mensaje = "El archivo no posee el campo Abreviación.";
                            return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                        }
                        // ******* Fin validación 

                        globales.query = $"select emp_id from dbo.farmacias_subnivel_encabezado where id={Convert.ToInt32(worksheet.Cells[2, 2].Text)}";
                        empId = sqlpg.getn(globales.query, conexion.Conectar());

                        globales.query = $"select * from dbo.farmacias_subnivel_detalle where fe_id={Convert.ToInt32(worksheet.Cells[2, 2].Text)}";
                        DataTable dtDetalle = new DataTable();
                        dtDetalle = sqlpg.ddt(globales.query, conexion.Conectar());

                        for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                        {
                            var cellValue = worksheet.Cells[row, 1].Text; // Leer el valor de la celda en la columna 1
                            foreach(DataRow item in dtDetalle.Rows)
                            {
                                if(item.Field<int>("consecutivo") == Convert.ToInt32(cellValue) && item.Field<string>("abreviacion") != worksheet.Cells[row, 5].Text)
                                {
                                    fd.Add(new FarmaciasSubnivelDetalle
                                    {
                                        fe_id = Convert.ToInt32(worksheet.Cells[row, 2].Text),
                                        consecutivo = Convert.ToInt32(worksheet.Cells[row, 1].Text),
                                        abreviacion = worksheet.Cells[row, 5].Text.Trim()
                                    });
                                }
                            }
                        }

                        // Ahora puedes hacer lo que quieras con los datos (por ejemplo, guardarlos en la base de datos)
                        // ...
                        
                        if(fd.Count > 0)
                        {
                            foreach (var item in fd)
                            {
                                globales.query = $"update dbo.farmacias_subnivel_detalle set abreviacion='{item.abreviacion}' where consecutivo={item.consecutivo} and fe_id={item.fe_id}";
                                sqlpg.save(globales.query, conexion.Conectar());
                            }

                            mensajeTipo = "exito";
                            mensaje = "Registros modificados satisfactoriamente.";
                        }
                        else
                        {
                            mensajeTipo = "aviso";
                            mensaje = "No se detecto cambios a realizar.";
                        }
                    }
                }
                else
                {
                    mensajeTipo = "error";
                    mensaje = "No se selecciono ningun archivo o el archivo esta vacio.";
                    return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
                }

                return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = empId });
            }
            catch(Exception ex)
            {
                string mensaje = "";
                string mensajeTipo = "";
                // Maneja la excepción, por ejemplo, mostrando un mensaje de error
                Console.WriteLine("Error al acceder a la hoja de cálculo: " + ex.Message);
                mensajeTipo = "error";
                mensaje = ex.Message;
                return RedirectToAction("Edit", "FarmaciasSubNivelesVisualizar", new { mensaje = mensaje, mensajeTipo = mensajeTipo, empId = 1 });
            }
        }
    }
}
