namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _001 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Areas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ConfCarpetaDetalles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NivelId = c.Int(nullable: false),
                        Descripcion = c.String(),
                        Longitud = c.Int(nullable: false),
                        AreaId = c.Int(nullable: false),
                        CarpetaEncabezadoid = c.Int(nullable: false),
                        Tipo_campoId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaEncabezadoes", t => t.CarpetaEncabezadoid, cascadeDelete: true)
                .ForeignKey("dbo.Tipo_Campo", t => t.Tipo_campoId, cascadeDelete: true)
                .Index(t => t.AreaId)
                .Index(t => t.CarpetaEncabezadoid)
                .Index(t => t.Tipo_campoId);
            
            CreateTable(
                "dbo.ConfCarpetaEncabezadoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        Subniveles = c.Int(nullable: false),
                        AreaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .Index(t => t.AreaId);
            
            CreateTable(
                "dbo.Tipo_Campo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(nullable: false, maxLength: 30),
                        Longitud = c.Int(nullable: false),
                        Automatico = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Documento_Detalle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        llaveUnica = c.String(nullable: false),
                        AreaId = c.Int(nullable: false),
                        CarpetaEncabezadoid = c.Int(nullable: false),
                        Nombre_Ori = c.String(nullable: false),
                        Nombre_Des = c.String(nullable: false),
                        Imagen = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaEncabezadoes", t => t.CarpetaEncabezadoid, cascadeDelete: true)
                .Index(t => t.AreaId)
                .Index(t => t.CarpetaEncabezadoid);
            
            CreateTable(
                "dbo.MantenimientoSubniveles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AreaId = c.Int(nullable: false),
                        CarpetaEncabezadoid = c.Int(nullable: false),
                        ConfCarpetaDetalleId = c.Int(nullable: false),
                        Descripcion = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Areas", t => t.AreaId, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaDetalles", t => t.ConfCarpetaDetalleId, cascadeDelete: true)
                .ForeignKey("dbo.ConfCarpetaEncabezadoes", t => t.CarpetaEncabezadoid, cascadeDelete: true)
                .Index(t => t.AreaId)
                .Index(t => t.CarpetaEncabezadoid)
                .Index(t => t.ConfCarpetaDetalleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.MantenimientoSubniveles", "CarpetaEncabezadoid", "dbo.ConfCarpetaEncabezadoes");
            DropForeignKey("dbo.MantenimientoSubniveles", "ConfCarpetaDetalleId", "dbo.ConfCarpetaDetalles");
            DropForeignKey("dbo.MantenimientoSubniveles", "AreaId", "dbo.Areas");
            DropForeignKey("dbo.Documento_Detalle", "CarpetaEncabezadoid", "dbo.ConfCarpetaEncabezadoes");
            DropForeignKey("dbo.Documento_Detalle", "AreaId", "dbo.Areas");
            DropForeignKey("dbo.ConfCarpetaDetalles", "Tipo_campoId", "dbo.Tipo_Campo");
            DropForeignKey("dbo.ConfCarpetaDetalles", "CarpetaEncabezadoid", "dbo.ConfCarpetaEncabezadoes");
            DropForeignKey("dbo.ConfCarpetaEncabezadoes", "AreaId", "dbo.Areas");
            DropForeignKey("dbo.ConfCarpetaDetalles", "AreaId", "dbo.Areas");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.MantenimientoSubniveles", new[] { "ConfCarpetaDetalleId" });
            DropIndex("dbo.MantenimientoSubniveles", new[] { "CarpetaEncabezadoid" });
            DropIndex("dbo.MantenimientoSubniveles", new[] { "AreaId" });
            DropIndex("dbo.Documento_Detalle", new[] { "CarpetaEncabezadoid" });
            DropIndex("dbo.Documento_Detalle", new[] { "AreaId" });
            DropIndex("dbo.ConfCarpetaEncabezadoes", new[] { "AreaId" });
            DropIndex("dbo.ConfCarpetaDetalles", new[] { "Tipo_campoId" });
            DropIndex("dbo.ConfCarpetaDetalles", new[] { "CarpetaEncabezadoid" });
            DropIndex("dbo.ConfCarpetaDetalles", new[] { "AreaId" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.MantenimientoSubniveles");
            DropTable("dbo.Documento_Detalle");
            DropTable("dbo.Tipo_Campo");
            DropTable("dbo.ConfCarpetaEncabezadoes");
            DropTable("dbo.ConfCarpetaDetalles");
            DropTable("dbo.Areas");
        }
    }
}
