using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    public class RolPermisos
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name = "Nombre del Rol")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name ="Pantalla")]
        public int IdPantalla { get; set; }
        [ForeignKey("IdPantalla")]
        public virtual Pantallas Pantallas { get; set; }
        [Required]
        public string Pantalla { get; set; }
        [Required]
        public string consultar { get; set; }
        [Required]
        public string crear { get; set; }
        [Required]
        public string editar { get; set; }
        [Required]
        public string eliminar { get; set; }
    }
}