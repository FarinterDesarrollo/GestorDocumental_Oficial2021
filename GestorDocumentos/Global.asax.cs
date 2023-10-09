using GestorDocumentos.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data;
using GestorDocumentos.Models.Request;

namespace GestorDocumentos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        Globales _globales = new Globales();
        Conexion _conexion = new Conexion();
        Sqlpg _oSqlpg = new Sqlpg();
        Sql _oSql = new Sql();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // ******* Precargar tipo de roles 21/09/2023 *******
            try
            {
                DataTable dtTipo = new DataTable();
                var myItem = new TipoRolItem();
                string tipo = ""; string descripcion = "";

                myItem.Item = new List<string>();
                myItem.Item.Add("A");

                for (int i = 0; i < myItem.Item.Count; i++)
                {
                    tipo = myItem.Item[i];
                    _globales.query = $"select * from dbo.tipo_rol where tipo in('{tipo}')";
                    dtTipo = _oSqlpg.ddt(_globales.query, _conexion.Conectar());
                    if (dtTipo.Rows.Count == 0)
                    {
                        if (tipo == "A")
                        {
                            descripcion = "Tiene acceso global, puede llevar restricciones de carpeta.";
                        }

                        _globales.query = $"INSERT INTO dbo.tipo_rol(tipo,descripcion)VALUES('{tipo}','{descripcion}')";
                        _oSqlpg.save(_globales.query, _conexion.Conectar());
                    }
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
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
