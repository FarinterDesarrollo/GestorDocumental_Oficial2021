namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _003 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleXAreaXCarpetas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        AreaId = c.Int(nullable: false),
                        CarpetaEncabezadoid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaEncabezadoes", t => t.CarpetaEncabezadoid, cascadeDelete: true)
                .Index(t => t.AreaId)
                .Index(t => t.CarpetaEncabezadoid);
            
            AlterColumn("dbo.RoleXAreas", "RoleName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleXAreaXCarpetas", "CarpetaEncabezadoid", "dbo.ConfCarpetaEncabezadoes");
            DropForeignKey("dbo.RoleXAreaXCarpetas", "AreaId", "dbo.Areas");
            DropIndex("dbo.RoleXAreaXCarpetas", new[] { "CarpetaEncabezadoid" });
            DropIndex("dbo.RoleXAreaXCarpetas", new[] { "AreaId" });
            AlterColumn("dbo.RoleXAreas", "RoleName", c => c.String());
            DropTable("dbo.RoleXAreaXCarpetas");
        }
    }
}
