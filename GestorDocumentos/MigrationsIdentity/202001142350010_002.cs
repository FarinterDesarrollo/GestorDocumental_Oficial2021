namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _002 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoleXAreas",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        AreaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .Index(t => t.AreaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoleXAreas", "AreaId", "dbo.Areas");
            DropIndex("dbo.RoleXAreas", new[] { "AreaId" });
            DropTable("dbo.RoleXAreas");
        }
    }
}
