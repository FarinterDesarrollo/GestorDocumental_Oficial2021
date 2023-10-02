using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    [Table("farmacias_subnivel_encabezado")]
    public class FarmaciasSubnivelEncabezado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        public short emp_id { get; set; }
        [Required]
        [StringLength(255)]
        public string emp_nombre { get; set; }
        [Required]
        [StringLength(255)]
        public string cadena_nombre { get; set; }
        [Required]
        [StringLength(255)]
        public string alias_cadena { get; set; }
    }
}