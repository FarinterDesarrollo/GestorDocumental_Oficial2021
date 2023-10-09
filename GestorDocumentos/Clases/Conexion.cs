using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Clases
{
    public class Conexion
    {
        public static string GD = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        public static string siteplus = ConfigurationManager.ConnectionStrings["SitePlusConnection"].ConnectionString;
        public string Conectar()
        {
            return GD;
        }

        public string ConecSitePlus()
        {
            return siteplus;
        }

    }
}