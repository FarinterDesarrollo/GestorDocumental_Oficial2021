using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    public class ConfCarpetaDetalle
    {
        public int Id { get; set; }
        public int NivelId { get; set; }
        public string Descripcion { get; set; }
        public int Longitud { get; set; }

        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public virtual Areas Areas { get; set; }

        public int CarpetaEncabezadoid { get; set;}
        [ForeignKey("CarpetaEncabezadoid")]
        public virtual ConfCarpetaEncabezado ConfCarpetaEncabezado { get; set; }

        public int Tipo_campoId { get; set; }       
        [ForeignKey("Tipo_campoId")]
        public virtual Tipo_Campo Tipo_Campo { get; set; }

    }
}