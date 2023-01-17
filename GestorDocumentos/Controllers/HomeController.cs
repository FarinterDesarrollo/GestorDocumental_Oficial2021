using GestorDocumentos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            //Agregado: 23/04/2020
            var chkTimeOut = Session.Timeout;
            //if (chkTimeOut < 20)
            //{
            //    // set new time out to session  
            //    Session.Timeout = 60;
            //    return View("TimeOut");
            //}
            //else
            //{
            //    return View("Demo");
            //}


            // ****************************************************************

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}