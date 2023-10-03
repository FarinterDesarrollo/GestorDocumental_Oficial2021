using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Request
{
    public class SucursalesLdcom
    {
        public short suc_id { get; set; }
        public string suc_nombre { get; set; }
        public string error { get; set; }
    }

    public class SucursalesAbreviacionLdcom
    {
        public short suc_id { get; set; }
        public string suc_nombre { get; set; }
        public string abreviacion { get; set; }
        public string error { get; set; }
    }
}