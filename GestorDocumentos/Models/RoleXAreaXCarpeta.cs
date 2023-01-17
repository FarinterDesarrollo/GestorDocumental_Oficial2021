using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    public class RoleXAreaXCarpeta
    {
        public int id { get; set; }
        [Required]
        [Display(Name = "Nombre del Rol")]
        public string RoleName { get; set; }
        [Required]
        [Display(Name = "Área")]
        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public virtual Areas Areas { get; set; }

        [Required]
        [Display(Name = "Nombre Carpeta")]
        public int CarpetaEncabezadoid { get; set; }
        [ForeignKey("CarpetaEncabezadoid")]
        public virtual ConfCarpetaEncabezado ConfCarpetaEncabezado { get; set; }
    }
}