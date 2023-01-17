using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GestorDocumentos.Models
{
    public class Tipo_Campo
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        [Display(Name ="Nombre del Tipo de Campo")]
        public string Descripcion { get; set; }
        [Required]
        [Display(Name ="Longitud")]
        public int Longitud { get; set; }
        [Required]
        [Display(Name ="Automático")]
        public int Automatico { get; set; }

    }
}