namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _008 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tipo_rol",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tipo = c.String(nullable: false, maxLength: 3),
                        descripcion = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tipo_rol");
        }
    }
}
