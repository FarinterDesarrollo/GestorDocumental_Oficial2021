using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GestorDocumentos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //Agregado: 24/04/2020
        //protected void Application_Start(object sender, EventArgs e)
        //{
        //    AreaRegistration.RegisterAllAreas();
        //    FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        //    RouteConfig.RegisterRoutes(RouteTable.Routes);
        //    BundleConfig.RegisterBundles(BundleTable.Bundles);

        //    Application["activeApplications"] = 0;
        //    Application["activeSessions"] = 0;

        //    Application["activeApplications"] = (int)Application["activeApplications"] + 1;
        //}

        //void Session_End(object sender, EventArgs e) {
        //    Application["activeSessions"] = (int)Application["activeSessions"] - 1;

        //    //if (Session["usuarioValido"] == null)
        //    //HttpContext.Current.Response.Redirect("~/Account/Login");

        //}




    }
}
