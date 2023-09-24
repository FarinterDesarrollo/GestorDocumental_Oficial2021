using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Request
{
    public class TipoRolResquest
    {
        public int id { get; set; }
        public string tipo { get; set; }
        public string descripcion { get; set; }
    }

    public partial class TipoRolItem
    {
        public List<string> Item { get; set; }
    }

    public partial class RolxTipoRequest
    {
        public string nombre { get; set; }
        public int idtipo { get; set; }
        public string tipo { get; set; }
        public string error { get; set; }
    }
}