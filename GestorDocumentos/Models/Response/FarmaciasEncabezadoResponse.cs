using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Response
{
    public class FarmaciasEncabezadoResponse
    {
        public int id { get; set; }
        public short empId { get; set; }
        public string empNombre { get; set; }
        public string cadenaNombre { get; set; }
        public string aliasCadena { get; set; }
        public string abreviacion { get; set; }
        public string error { get; set; }
    }
}