using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Request
{
    public class FarmaciasEncabezadoRequest
    {
        public short empId { get; set; }
        public string empNombre { get; set; }
        public string cadenaNombre { get; set; }
        public string aliasCadena { get; set; }
        public string abreviacion { get; set; }
        public string baseDatos { get; set; }
    }

    public partial class FarmaciasSubNivelesRequest
    {
        public FarmaciasSubnivelEncabezado farmaciasEncabezado { get; set; }
        public List<FarmaciasSubnivelDetalle> farmaciasDetalle { get; set; }
    }

    public partial class EncabezadoFields
    {
        public int id { get; set; }
    }
}