using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using GestorDocumentos.Models;
using System.Collections.Generic;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class EditarUsuarioController : Controller 
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        //public EditarUsuarioController()
        //{
        //}

        //public EditarUsuarioController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        //{
        //    UserManager = userManager;
        //    RoleManager = roleManager;
        //}

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

        //public ApplicationRoleManager RoleManager
        //{
        //    get
        //    {
        //        return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
        //    }
        //    private set
        //    {
        //        _roleManager = value;
        //    }
        //}


        // GET: EditarUsuario
        public ActionResult Index()
        {
            return View();
        }

        // GET: /EditarUsuario/ListaDeUsuariosEdit
        public ActionResult ListaDeUsuariosEdit(string nombre)
        {
            var context = HttpContext.GetOwinContext().Get<ApplicationDbContext>();

            if (!String.IsNullOrEmpty(nombre))
            {
                List<EditarUsuario> usuarios = (from u in context.Users
                                                      from ur in u.Roles
                                                      join r in context.Roles on ur.RoleId equals r.Id
                                                      select new EditarUsuario()
                                                      {
                                                          Id=u.Id.ToString(),
                                                          Email = u.Email,
                                                          RoleName = r.Name
                                                      }).ToList();

                usuarios = usuarios.Where(x => x.Email.Trim().Contains(nombre)).ToList();

                if (usuarios.Count == 0)
                {
                    Request.Flash("warning", "¡El usuario especificado no existe, favor escribalo con un formato de correo!");
                }

                return View(usuarios.ToList());
            }
            else
            {
                List<EditarUsuario> usuarios = (from u in context.Users
                                                from ur in u.Roles
                                                join r in context.Roles on ur.RoleId equals r.Id
                                                select new EditarUsuario()
                                                {
                                                    Id = u.Id.ToString(),
                                                    Email = u.Email,
                                                    RoleName = r.Name
                                                }).ToList();

                return View(usuarios.ToList());
            }
        }

        //Opcion 2 de editar:

        public JsonResult RolExistente()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var role in db.Roles)
                list.Add(new SelectListItem() { Value = role.Id, Text = role.Name });
            ViewBag.Roles = list;
            //return View(list.ToList());
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // ************************************************************************

        // GET: EditarUsuario/Editar
        //public ActionResult Editar(string id, string rol)
        //{
        //    if (!String.IsNullOrEmpty(id))
        //    {
        //        try
        //        {
        //            var userReloaded = db.Users.Where(u => u.Id == id).FirstOrDefault();
        //            userReloaded.Roles.Clear();
        //            var result =  UserManager.AddToRoleAsync(id,rol);
        //            //var result2 = UserManager.UpdateAsync(userReloaded);

        //            if (result.IsCompleted)
        //            {
        //                db.SaveChanges();
        //            }

        //            var userVm = new
        //            {
        //                Id = id,
        //                RoleName = rol
        //            };

        //            return Json(new { Result = "OK", Data = userVm, Message = "Cambios Guardados." });

        //        }
        //        catch (Exception e)
        //        {
        //            return Json(new { Result = "ERROR", Message = e.Message });
        //        }
        //    }

        //    else
        //    {
        //        return HttpNotFound();
        //    }

        //}

        //[HttpPost]
        public async Task<ActionResult> Editar(string id, string rol,string rola)
        {

           var result = await UserManager.AddToRoleAsync(id, rol);
           var result2 = await UserManager.RemoveFromRoleAsync(id, rola);

            return RedirectToAction("Editar", new { Id = rol });
        }

    }
}