using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    [Table("rolxtipo")]
    public class RolxTipo
    {
        [Key]
        [Required]
        [Display(Name = "rolnombre")]
        public string rolnombre { get; set; }
        [Display(Name = "idtipo")]
        public int idtipo { get; set; }
        [ForeignKey("idtipo")]
        public virtual TipoRol TipoRol { get; set; }
    }
}