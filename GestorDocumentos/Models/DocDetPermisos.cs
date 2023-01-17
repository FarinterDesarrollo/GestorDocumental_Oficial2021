using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestorDocumentos.Models;
using System.Net;
namespace GestorDocumentos.Models
{
    public class DocDetPermisos
    {
        public int Id { get; set; }
        public string area { get; set; }
        public string carpeta { get; set; }
        public string llaveunica { get; set; }
        public string Nombreori { get; set; }
        public string Nombredes { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }

    }
}