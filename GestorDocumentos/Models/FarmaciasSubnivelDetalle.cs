using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    [Table("farmacias_subnivel_detalle")]
    public class FarmaciasSubnivelDetalle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int consecutivo { get; set; }
        [Key]
        [Column(Order = 1)]
        public int fe_id { get; set; }
        [Key]
        [Column(Order = 2)]
        public short suc_id { get; set; }
        [Required]
        [StringLength(60)]
        public string suc_nombre { get; set; }
        [Required]
        [StringLength(10)]
        public string abreviacion { get; set; }
    }
}