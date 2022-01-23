namespace WebAppAspNetMvcIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Type, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Settings", new[] { "Type" });
            DropTable("dbo.Settings");
        }
    }
}
