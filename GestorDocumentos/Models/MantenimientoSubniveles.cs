using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    public class MantenimientoSubniveles
    {
        public int Id { get; set; }

        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public virtual Areas Areas { get; set; }

        public int CarpetaEncabezadoid { get; set; }
        [ForeignKey("CarpetaEncabezadoid")]
        public virtual ConfCarpetaEncabezado ConfCarpetaEncabezado { get; set; }

        public int ConfCarpetaDetalleId { get; set; }
        [ForeignKey("ConfCarpetaDetalleId")]
        public virtual ConfCarpetaDetalle ConfCarpetaDetalle { get; set; }

        public string Descripcion { get; set; }
    }
}