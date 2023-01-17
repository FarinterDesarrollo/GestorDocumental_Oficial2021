using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace GestorDocumentos.Models
{
    public class EditarUsuario
    {
        public string Id { get; set; }
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
        public string RoleId { get; set; }
        [Display(Name = "Rol")]
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }


    //public class Edicion : DbContext
    //{
    //    public DbSet<EditarUsuario> users { get; set; }
    //}
}