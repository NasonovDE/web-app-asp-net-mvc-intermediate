namespace WebAppAspNetMvcIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Films", "FilmDescription", c => c.String(nullable: false));
            AddColumn("dbo.Films", "FilmAllActors", c => c.String(nullable: false));
            AddColumn("dbo.Films", "FilmDop", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Films", "FilmDop");
            DropColumn("dbo.Films", "FilmAllActors");
            DropColumn("dbo.Films", "FilmDescription");
        }
    }
}
