using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Request
{
    public class EmpresaLdcom
    {
        public short emp_id { get; set; }
        public string emp_nombre { get; set; }
        public string error { get; set; }
    }
}