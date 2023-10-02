namespace GestorDocumentos.MigrationsIdentity
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _010 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.farmacias_subnivel_detalle",
                c => new
                    {
                        fe_id = c.Int(nullable: false),
                        suc_id = c.Short(nullable: false),
                        consecutivo = c.Int(nullable: false, identity: true),
                        suc_nombre = c.String(nullable: false, maxLength: 60),
                        abreviacion = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => new { t.fe_id, t.suc_id });
            
            CreateTable(
                "dbo.farmacias_subnivel_encabezado",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        emp_id = c.Short(nullable: false),
                        emp_nombre = c.String(nullable: false, maxLength: 255),
                        cadena_nombre = c.String(nullable: false, maxLength: 255),
                        alias_cadena = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.farmacias_subnivel_encabezado");
            DropTable("dbo.farmacias_subnivel_detalle");
        }
    }
}
