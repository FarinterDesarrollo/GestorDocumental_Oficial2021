using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GestorDocumentos.Models.Response
{
    public class EmpXCadenaResponse
    {
        public short empId { get; set; }
        public string empNombre { get; set; }
        public string cadenaNombre { get; set; }
        public string notacion { get; set; }
    }

    public partial class ErrorResponse
    {
        public string error { get; set; }
    }
}