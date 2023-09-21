namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _009 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.rolxtipo",
                c => new
                    {
                        rolnombre = c.String(nullable: false, maxLength: 128),
                        idtipo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.rolnombre)
                .ForeignKey("dbo.tipo_rol", t => t.idtipo, cascadeDelete: true)
                .Index(t => t.idtipo);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.rolxtipo", "idtipo", "dbo.tipo_rol");
            DropIndex("dbo.rolxtipo", new[] { "idtipo" });
            DropTable("dbo.rolxtipo");
        }
    }
}
