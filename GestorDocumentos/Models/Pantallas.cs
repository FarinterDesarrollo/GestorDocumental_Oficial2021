using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    public class Pantallas
    {
        [Key]
        public int IdPantalla { get; set; }
        [Required]
        [Display(Name = "Pantalla")]
        public string pantalla { get; set; }
    }
}