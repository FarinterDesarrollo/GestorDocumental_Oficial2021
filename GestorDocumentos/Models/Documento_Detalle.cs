using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GestorDocumentos.Models
{
    public class Documento_Detalle
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Llave Única")]
        public string llaveUnica { get; set; }

        [Required]
        [Display(Name ="Área")]
        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public virtual Areas Areas { get; set; }

        [Required]
        [Display(Name = "Nombre Carpeta")]
        public int CarpetaEncabezadoid { get; set; }
        [ForeignKey("CarpetaEncabezadoid")]
        public virtual ConfCarpetaEncabezado ConfCarpetaEncabezado { get; set; }

        //[Required]
        //[Display(Name = "Nombre Nivel")]
        //public int ConfCarpetaDetalleid { get; set; }
        //[ForeignKey("ConfCarpetaDetalleid")]
        //public virtual ConfCarpetaDetalle ConfCarpetaDetalle { get; set; }

        //[Required]
        //[Display(Name = "Nombre Sub Nivel")]
        //public int MantenimientoSubnivelesid { get; set; }
        //[ForeignKey("MantenimientoSubnivelesid")]
        //public virtual MantenimientoSubniveles MantenimientoSubniveles { get; set; }

        [Required]
        [Display(Name = "Nombre Original")]
        public string Nombre_Ori { get; set; }

        [Required]
        [Display(Name = "Nombre Destino")]
        public string Nombre_Des { get; set; }

        [Required]
        [Display(Name ="Imagen")]
        public byte[] Imagen { get; set; }
        
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaActualizacion { get; set; }

        public string RoleName { get; set; }
    }
}