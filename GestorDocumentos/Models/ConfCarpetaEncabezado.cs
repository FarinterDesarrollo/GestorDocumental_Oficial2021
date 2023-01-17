using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models
{
    public class ConfCarpetaEncabezado
    {
        public int Id { get; set; }
        public string  Descripcion { get; set; }
        public int Subniveles { get; set; }

        public int AreaId { get; set; }
        [ForeignKey("AreaId")]
        public virtual Areas Areas { get; set; }
    }
}