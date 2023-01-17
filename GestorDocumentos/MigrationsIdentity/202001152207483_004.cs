namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _004 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleXAreaXCarpetasXSubniveles",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        AreaId = c.Int(nullable: false),
                        CarpetaEncabezadoid = c.Int(nullable: false),
                        ConfCarpetaDetalleid = c.Int(nullable: false),
                        MantenimientoSubnivelesid = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaDetalles", t => t.ConfCarpetaDetalleid, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaEncabezadoes", t => t.CarpetaEncabezadoid, cascadeDelete: true)
                .ForeignKey("dbo.MantenimientoSubniveles", t => t.MantenimientoSubnivelesid, cascadeDelete: true)
                .Index(t => t.AreaId)
                .Index(t => t.CarpetaEncabezadoid)
                .Index(t => t.ConfCarpetaDetalleid)
                .Index(t => t.MantenimientoSubnivelesid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleXAreaXCarpetasXSubniveles", "MantenimientoSubnivelesid", "dbo.MantenimientoSubniveles");
            DropForeignKey("dbo.RoleXAreaXCarpetasXSubniveles", "CarpetaEncabezadoid", "dbo.ConfCarpetaEncabezadoes");
            DropForeignKey("dbo.RoleXAreaXCarpetasXSubniveles", "ConfCarpetaDetalleid", "dbo.ConfCarpetaDetalles");
            DropForeignKey("dbo.RoleXAreaXCarpetasXSubniveles", "AreaId", "dbo.Areas");
            DropIndex("dbo.RoleXAreaXCarpetasXSubniveles", new[] { "MantenimientoSubnivelesid" });
            DropIndex("dbo.RoleXAreaXCarpetasXSubniveles", new[] { "ConfCarpetaDetalleid" });
            DropIndex("dbo.RoleXAreaXCarpetasXSubniveles", new[] { "CarpetaEncabezadoid" });
            DropIndex("dbo.RoleXAreaXCarpetasXSubniveles", new[] { "AreaId" });
            DropTable("dbo.RoleXAreaXCarpetasXSubniveles");
        }
    }
}
