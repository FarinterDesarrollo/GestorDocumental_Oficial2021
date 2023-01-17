using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Text.RegularExpressions;
using GestorDocumentos.Models;
using System.Collections.Generic;
using ExcelDataReader;
using System.Data;
using GestorDocumentos.Clases;
using System.Net;

namespace GestorDocumentos.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private ApplicationDbContext db = new ApplicationDbContext();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
            // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            model.malsesion = result.ToString();
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                //return RedirectToAction("Index", "Role");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
                    // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");

                    return RedirectToAction("Lista_RolesxUsuario", "Account");
                }

                //Regex pattern = new Regex(@"^[0-9A-Z]([-.\w]*[0-9A-Z])*$");
                Match alfanum = Regex.Match(model.Password, @"^[A-Za-z0-9\#$%&/()=_^*¡!'.+-¿?,]*$", RegexOptions.IgnoreCase);

                if (alfanum.Success)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error + " Ejemplo: Usuario00*");
                    }
                }
                else
                {
                    AddErrors(result);
                }

                //AddErrors(result);
            }
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        // Agregado: 08/05/2020 *************************************************************************
        
        // Agregado: 08/06/2022 *************************************************************************
        [AllowAnonymous]
        public ActionResult Register2()
        {
            try
            {
                List<SelectListItem> list = new List<SelectListItem>();
                foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                    list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
                ViewBag.Roles = list;

                // *** Variables de estado ***

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

                string Excel_Email = Session["Excel_Email"] as string;
                if (Excel_Email == "Y")
                {
                    ViewBag.Excel_Email = Excel_Email;
                    Session["Excel_Email"] = "";
                }

                string Excel_EmailRepetidos = Session["Excel_EmailRepetidos"] as string;
                if (Excel_EmailRepetidos == "Y")
                {
                    ViewBag.Excel_EmailRepetidos = Excel_EmailRepetidos;
                    Session["Excel_EmailRepetidos"] = "";
                }

                string Excel_Password = Session["Excel_Password"] as string;
                if (Excel_Password == "Y")
                {
                    ViewBag.Excel_Password = Excel_Password;
                    Session["Excel_Password"] = "";
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register2(RegisterViewModel model, HttpPostedFileBase file)
        {
            try
            {
                if (model.RoleName != "")
                {
                    // --- Creación del archivo de excel
                    // Validar Archivo
                    if (file != null && file.ContentLength > 0)
                    {
                        string AspNetUsers = "-AspNetUsers-";
                        AspNetUsers = AspNetUsers.Replace('-', '"');
                        string Email = "-Email-";
                        Email = Email.Replace('-', '"');

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
                        if ((tabla.Columns.Count < 2) || (tabla.Columns.Count > 2))
                        {
                            Session["Excel_Columnas"] = "Y"; //Variable global columnas requeridas "Y" si no cumple
                            // Mensaje
                            goto y;
                        }

                        //Validación si el archivo de excel no contiene el nombre de columna requerido
                        if ((tabla.Columns[0].ColumnName != "Email") || (tabla.Columns[1].ColumnName != "Password"))
                        {
                            Session["Excel_NombreColumna"] = "Y"; //Variable global nombre columna requeridas "Y" si no cumple
                            // Mensaje
                            goto y;
                        }

                        //Validación si el archivo de excel contiene datos repetidos, columna email
                        var ColumnaEmail = new List<string>();
                        var ColumnaPassword = new List<string>();
                        foreach (DataRow k in tabla.Rows)
                        {
                            ColumnaEmail.Add(k[0].ToString());
                            ColumnaPassword.Add(k[1].ToString());
                        }

                        // Columna Email
                        var x = from ob in ColumnaEmail group ob by ob into g select new { Name = g.Key, Duplicatecount = g.Count() };
                        foreach (var m in x)
                        {
                            Console.WriteLine(m.Name + "--" + m.Duplicatecount);
                            if ((m.Name == null) || (m.Name == ""))
                            {
                                Session["Excel_Email"] = "Y"; //Variable global email "Y" si no contiene valores
                                // Mensaje
                                goto y;
                            }

                            // Verifica si el email no contiene caracteres especiales
                            //var withoutSpecial = new string(m.Name.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c) || Char.IsPunctuation(c)).ToArray());
                            //string vname = m.Name.Replace(" ","");
                            //string vname = m.Name;
                            // Si tiene caracteres especiales
                            //if (vname != withoutSpecial)
                            //{
                            //    Session["Excel_MatCarEsp"] = "Y"; //Variable global caracteres especiales "Y" si contiene 
                            //    return RedirectToAction("Index", "VSP_Casa_ClientePrv_Sociedad", new { CodCasa = codcasa });
                            //}

                            // Nombres repetidos
                            if (m.Duplicatecount > 1)
                            {
                                Session["Excel_EmailRepetidos"] = "Y"; //Variable global repetidos "Y" si contiene 
                                // Mensaje
                                goto y;
                            }
                        }

                        // Columna Email
                        var z = from ob in ColumnaPassword group ob by ob into g select new { Name = g.Key, Duplicatecount = g.Count() };
                        foreach (var m in x)
                        {
                            Console.WriteLine(m.Name + "--" + m.Duplicatecount);
                            if ((m.Name == null) || (m.Name == ""))
                            {
                                Session["Excel_Password"] = "Y"; //Variable global password "Y" si no contiene valores
                                // Mensaje
                                goto y;
                            }
                        }
                        // *********** Inicio Proceso Final de validación e inserción de datos ***********
                        List<Cls_DatosExcel_Detalle> DatosExcel = new List<Cls_DatosExcel_Detalle>();
                        string email = ""; string password = "";

                        foreach (DataRow dr in dataSet.Tables[0].Rows) // Validación si esta registrado el email
                        {
                            //Muestras los valores obteniendolos con el Índice o el Nombre de la columna, 
                            //de la siguiente manera:

                            email = dr["Email"].ToString();
                            password = dr["Password"].ToString();

                            email = email.Trim();
                            password = password.Trim();

                            var MQuery = db.Database.SqlQuery<Cls_DatosExcel_Detalle>("SELECT " + Email + " FROM dbo." + AspNetUsers + " WHERE " + Email + "='" + email + "'").ToList();

                            if (MQuery.Count == 0)
                            {
                                DatosExcel.Add(new Cls_DatosExcel_Detalle
                                {
                                    Email = email,
                                    Password = password,
                                    Status = "Exitoso"
                                });
                            }
                            else
                            {
                                DatosExcel.Add(new Cls_DatosExcel_Detalle
                                {
                                    Email = email,
                                    Password = password,
                                    Status = "Ya existe en tabla AspNetUsers"
                                });
                            }
                        }

                        foreach (var item in DatosExcel)
                        {
                            if(item.Status == "Exitoso")
                            {
                                //Guardar información en la tabla usuarios
                                model.Email = item.Email;
                                model.Password = item.Password;
                                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                                var result = await UserManager.CreateAsync(user, model.Password);

                                if (result.Succeeded)
                                {
                                    result = await UserManager.AddToRoleAsync(user.Id, model.RoleName);
                                    Session["Excel_Cargado"] = "Y"; //Variable global Excel cargado "Y" se cargo 
                                }
                                else
                                {
                                    //Regex pattern = new Regex(@"^[0-9A-Z]([-.\w]*[0-9A-Z])*$");
                                    Match alfanum = Regex.Match(model.Password, @"^[A-Za-z0-9\#$%&/()=_^*¡!'.+-¿?,]*$", RegexOptions.IgnoreCase);

                                    if (alfanum.Success)
                                    {
                                        foreach (var error in result.Errors)
                                        {
                                            ModelState.AddModelError("", error + " Ejemplo: Usuario00*");
                                            item.Status = error + ", Ejemplo: Usuario00*";
                                        }
                                    }
                                    else
                                    {
                                        AddErrors(result);
                                    }
                                }
                            }
                        }

                        var myExport = new CsvExport();
                        foreach (var items in DatosExcel)
                        {
                            myExport.AddRow();
                            myExport["Email"] = items.Email;
                            myExport["Status"] = items.Status;
                        }

                        byte[] myCsvData = myExport.ExportToBytes();

                        //// Obtiene la respuesta actual
                        System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                        // Tipo de contenido para forzar la descarga
                        response.ContentType = "application/octet-stream";
                        response.AddHeader("Content-Disposition", "attachment; filename=" + "Estado de carga usuarios " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.InvariantCulture) + ".csv");
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Session["Excel_Error"] = "Y";
            }
            y:
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;
            return Redirect("Register2");
            //return View(model);
        }

        //Clase paramétros a cargar
        private class Cls_DatosExcel_Detalle
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Status { get; set; }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Descargar_Formato()
        {
            try
            {
                var myExport = new CsvExport();
                string Email = ""; string Password = "";
                myExport.AddRow();
                myExport["Email"] = Email;
                myExport["Password"] = Password;

                byte[] myCsvData = myExport.ExportToBytes();

                //// Obtiene la respuesta actual
                System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
                // Tipo de contenido para forzar la descarga
                response.ContentType = "application/vnd.ms-excel";
                response.AddHeader("Content-Disposition", "attachment; filename=" + "Formato de carga usuarios " + DateTime.Now.ToString("dd/MMM/yyyy hh:mm tt", CultureInfo.InvariantCulture) + ".xls");
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return Redirect("Register2");
        }
        // Fin agregado

        // GET: /Account/Register
        [HttpGet]
        public ActionResult Edit(string email)
        {
            ApplicationDbContext db = new ApplicationDbContext();
        
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in RoleManager.Roles.OrderBy(x => x.Name))
                list.Add(new SelectListItem() { Value = role.Name, Text = role.Name });
            ViewBag.Roles = list;

            //Modificado Septiembre 2020:
            Session["AUsuarioId"] = email;

            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            var users = (from u in context.Users
                         from ur in u.Roles
                         join r in context.Roles on ur.RoleId equals r.Id
                         where u.UserName == email
                         select new RegisterViewModel()
                         {
                             Email = u.Email,
                             RoleName = r.Name
                         });

            List<SelectListItem> lst = new List<SelectListItem>();
            foreach (var uxr in users)
                lst.Add(new SelectListItem() { Value = uxr.RoleName, Text = uxr.RoleName });
            ViewBag.RActual = lst[0].Text;

            return View(users);
        }


        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(string email, string Rol)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            var users = (from u in context.Users
                         from ur in u.Roles
                         join r in context.Roles on ur.RoleId equals r.Id
                         where u.UserName == email
                         select new {u.Id,r.Name }).ToList();
            string idusu = users[0].Id;
            string role = Rol;
            string erol = users[0].Name;

            if (erol != Rol)
            {
                var result2 = await UserManager.RemoveFromRoleAsync(idusu, erol);
            }


            var result = await UserManager.AddToRoleAsync(idusu, role);

            Request.Flash("success", "¡Rol Cambiado correctamente!");
            return RedirectToAction("Delete", "Account");
        }

        public JsonResult GetUserIdCPW(string email)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            List<SelectListItem> users = (from u in context.Users
                         from ur in u.Roles
                         join r in context.Roles on ur.RoleId equals r.Id
                         where u.UserName == email
                         select new SelectListItem { Value= u.Id.ToString(), Text=u.UserName }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserIdCPW2(string id)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            List<SelectListItem> users = (from u in context.Users
                                          from ur in u.Roles
                                          join r in context.Roles on ur.RoleId equals r.Id
                                          where u.Id == id
                                          select new SelectListItem { Value = u.Id.ToString(), Text = u.UserName }).ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ChangePassword(string email) {
            //Modificado Septiembre 2020:
            Session["AUsuarioId"] = email;
            ViewBag.LUsuario = email;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //var result = await UserManager.ChangePasswordAsync(model.id, model.OldPassword, model.NewPassword);
            var result = await UserManager.ChangePasswordAsync(model.id, model.NewPassword, model.ConfirmPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }

                Request.Flash("success", "¡Contraseña Cambiada Correctamente!");
                return RedirectToAction("Delete", new { Message = ManageController.ManageMessageId.ChangePasswordSuccess });
            }

            Request.Flash("warning", "¡Se Produjo un Error al Cambiar la Contraseña, Verifique su contraseña actual si coincide con lo escrito!");
            AddErrors(result);
            return View(model);
        }


        //******************************************************************* Agregado ^

        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Solicitar redireccionamiento al proveedor de inicio de sesión externo
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si el usuario no tiene ninguna cuenta, solicitar que cree una
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //14/01/2020:
        // GET: /Admin/UserAssignRole
        [HttpGet]
        //[ValidateAntiForgeryToken]
        public ActionResult Lista_RolesxUsuario(ListaRolesxUsuarios vm)
        {
            //ApplicationDbContext context = new ApplicationDbContext();
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            var users = (from u in context.Users
                         from ur in u.Roles
                         join r in context.Roles on ur.RoleId equals r.Id
                         select new ListaRolesxUsuarios()
                         {
                             UserName = u.UserName,
                             Email = u.Email,
                             UserRoles = r.Name
                         });
            return View(users);
        }

        //Agregado: 17-03-2020
        //Eliminar usuario

        public ActionResult Delete(string eliminar,string Nombre)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            //Modificado Septiembre 2020:
            string valor = Session["AUsuarioId"] as string;
            if (valor != null && valor != "")
            {
                Nombre = valor;
                Session["AUsuarioId"] = "";
            }

            if (!String.IsNullOrEmpty(Nombre))
            {
                List<ListaRolesxUsuarios> usuarios = (from u in context.Users
                                                   from ur in u.Roles
                                                   join r in context.Roles on ur.RoleId equals r.Id
                                                   select new ListaRolesxUsuarios()
                                                   {
                                                       UserName = u.UserName,
                                                       Email = u.Email,
                                                       UserRoles = r.Name
                                                   }).ToList();

                usuarios= usuarios.Where(x => x.Email.Trim().Contains(Nombre)).ToList();

                if (usuarios.Count ==0)
                {
                    Request.Flash("warning", "¡El usuario especificado no existe o este fue eliminado, favor escribalo con un formato de correo!");
                }

                if (eliminar != null)
                {
                    DeleteConfirmed(Nombre);
                }

                return View(usuarios.ToList());
            }

            else
            {
                List<ListaRolesxUsuarios> usuarios = (from u in context.Users
                                                   from ur in u.Roles
                                                   join r in context.Roles on ur.RoleId equals r.Id
                                                   select new ListaRolesxUsuarios()
                                                   {
                                                       UserName = u.UserName,
                                                       Email = u.Email,
                                                       UserRoles = r.Name
                                                   }).ToList();
                return View(usuarios.ToList());
            }

        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string theName)
        {
            using (var userContext = new ApplicationDbContext())
            {
                // Validación de existencia:
                var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

                if(theName == null)
                {
                    List<ListaRolesxUsuarios> usuarios = (from u in context.Users
                                                          from ur in u.Roles
                                                          join r in context.Roles on ur.RoleId equals r.Id
                                                          select new ListaRolesxUsuarios()
                                                          {
                                                              UserName = u.UserName,
                                                              Email = u.Email,
                                                              UserRoles = r.Name
                                                          }).ToList();
                    return View(usuarios.ToList());
                }
                else
                {
                    List<ListaRolesxUsuarios> usuarios = (from u in context.Users
                                                          from ur in u.Roles
                                                          join r in context.Roles on ur.RoleId equals r.Id
                                                          select new ListaRolesxUsuarios()
                                                          {
                                                              UserName = u.UserName,
                                                              Email = u.Email,
                                                              UserRoles = r.Name
                                                          }).ToList();

                    usuarios = usuarios.Where(x => x.Email.Trim().Contains(theName)).ToList();

                    if (usuarios.Count == 0)
                    {
                        Request.Flash("danger", "¡El usuario especificado no existe o cancelo la eliminación del mismo!");
                        return View(usuarios.ToList());
                    }
                }

                // ***************************************************************************
                var objUser = (from p in userContext.Users
                               where p.Email == theName
                               select p).FirstOrDefault();

                userContext.Users.Remove(objUser);

                userContext.SaveChanges();



                //List<ListaRolesxUsuarios> usuariosdel = (from u in context.Users
                //                                      from ur in u.Roles
                //                                      join r in context.Roles on ur.RoleId equals r.Id
                //                                      select new ListaRolesxUsuarios()
                //                                      {
                //                                          UserName = u.UserName,
                //                                          Email = u.Email,
                //                                          UserRoles = r.Name
                //                                      }).ToList();
                //usuariosdel = usuariosdel.Where(x => x.Email.Trim().Contains(theName)).ToList();

                Request.Flash("danger", "¡El usuario ha sido eliminado correctamente!");
                //return View(usuariosdel.ToList());

                return RedirectToAction("Delete", "Account");
            }
        }

        // ****************************************************************************************

        // Agregado 21-03-2020


        // ****************************************************************************************

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}