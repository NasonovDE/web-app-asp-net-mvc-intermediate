namespace WebAppAspNetMvcIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init9 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.FilmCovers", "Size");
            DropColumn("dbo.FilmCovers", "Path");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FilmCovers", "Path", c => c.String());
            AddColumn("dbo.FilmCovers", "Size", c => c.Long(nullable: false));
        }
    }
}
