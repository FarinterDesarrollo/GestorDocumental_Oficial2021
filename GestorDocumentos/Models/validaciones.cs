using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GestorDocumentos.Models;

namespace GestorDocumentos.Models
{
    public class validaciones
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        public string Longitud { get; set; }
        public Exception error { get; set; }
        public string confirmacion { get; set; }

        public async System.Threading.Tasks.Task validarlongitudAsync(string id)
        {
            try
            {
                if (id == null)
                {
                    this.confirmacion = "Envie un valor valido";
                }
                Tipo_Campo  tipo_campo = await db.Tipo_Campo.FindAsync(id);
                if (tipo_campo == null)
                {
                    this.confirmacion = "No existen datos a mostrar";
            
                }
            
            }
            catch (Exception ex)
            {
                this.error = ex;
            }
        }

    }
}