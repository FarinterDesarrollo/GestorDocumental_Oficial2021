namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _005 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Pantallas",
                c => new
                    {
                        IdPantalla = c.Int(nullable: false, identity: true),
                        pantalla = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.IdPantalla);
            
            CreateTable(
                "dbo.RolPermisos",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        IdPantalla = c.Int(nullable: false),
                        Pantalla = c.String(),
                        consultar = c.String(),
                        crear = c.String(),
                        editar = c.String(),
                        eliminar = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Pantallas", t => t.IdPantalla, cascadeDelete: true)
                .Index(t => t.IdPantalla);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RolPermisos", "IdPantalla", "dbo.Pantallas");
            DropIndex("dbo.RolPermisos", new[] { "IdPantalla" });
            DropTable("dbo.RolPermisos");
            DropTable("dbo.Pantallas");
        }
    }
}
