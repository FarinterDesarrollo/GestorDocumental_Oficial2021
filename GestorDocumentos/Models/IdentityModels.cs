using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
namespace GestorDocumentos.Models
{
    // Para agregar datos de perfil del usuario, agregue más propiedades a su clase ApplicationUser. Visite https://go.microsoft.com/fwlink/?LinkID=317594 para obtener más información.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        public DbSet<ConfCarpetaEncabezado> ConfCarpetaEncabezados { get; set; }
        public DbSet<ConfCarpetaDetalle> ConfCarpetaDetalle { get; set; }
        //public DbSet<Documentos_Encabezado> Documentos_Encabezados { get; set; }
        public DbSet<Documento_Detalle> Documentos_Detalle { get; set; }
        public DbSet<MantenimientoSubniveles> MantenimientoSubniveles { get; set; }
        public System.Data.Entity.DbSet<GestorDocumentos.Models.Tipo_Campo> Tipo_Campo { get; set; }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<GestorDocumentos.Models.Areas> Areas { get; set; }

        public System.Data.Entity.DbSet<GestorDocumentos.Models.RoleXArea> RoleXAreas { get; set; }

        public System.Data.Entity.DbSet<GestorDocumentos.Models.RoleXAreaXCarpeta> RoleXAreaXCarpetas { get; set; }

        public System.Data.Entity.DbSet<GestorDocumentos.Models.RoleXAreaXCarpetasXSubniveles> RoleXAreaXCarpetasXSubniveles { get; set; }

        public System.Data.Entity.DbSet<GestorDocumentos.Models.Pantallas> Pantallas { get; set; }

        public System.Data.Entity.DbSet<GestorDocumentos.Models.RolPermisos> RolPermisos { get; set; }

        //Agregado el 20/09/2023
        public System.Data.Entity.DbSet<GestorDocumentos.Models.TipoRol> TipoRol { get; set; }
        public System.Data.Entity.DbSet<GestorDocumentos.Models.RolxTipo> RolxTipo { get; set; }
        //Agregado el 25/09/2026
        public System.Data.Entity.DbSet<GestorDocumentos.Models.FarmaciasSubnivelEncabezado> FarmaciasSubnivelEncabezado { get; set; }
        public System.Data.Entity.DbSet<GestorDocumentos.Models.FarmaciasSubnivelDetalle> FarmaciasSubnivelDetalle { get; set; }
        //public System.Data.Entity.DbSet<GestorDocumentos.Models.RoleViewModels> RoleViewModels { get; set; }
    }
}