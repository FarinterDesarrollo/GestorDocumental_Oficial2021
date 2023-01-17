namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _006 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RolPermisos", "Pantalla", c => c.String(nullable: false));
            AlterColumn("dbo.RolPermisos", "consultar", c => c.String(nullable: false));
            AlterColumn("dbo.RolPermisos", "crear", c => c.String(nullable: false));
            AlterColumn("dbo.RolPermisos", "editar", c => c.String(nullable: false));
            AlterColumn("dbo.RolPermisos", "eliminar", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RolPermisos", "eliminar", c => c.String());
            AlterColumn("dbo.RolPermisos", "editar", c => c.String());
            AlterColumn("dbo.RolPermisos", "crear", c => c.String());
            AlterColumn("dbo.RolPermisos", "consultar", c => c.String());
            AlterColumn("dbo.RolPermisos", "Pantalla", c => c.String());
        }
    }
}
