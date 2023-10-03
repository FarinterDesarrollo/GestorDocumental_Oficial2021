using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Request
{
    public class CadenaLdcom
    {
        public short cadena_id { get; set; }
        public string cadena_nombre { get; set; }
        public string error { get; set; }
    }
}