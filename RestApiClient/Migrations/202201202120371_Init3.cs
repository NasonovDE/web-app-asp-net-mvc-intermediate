namespace WebAppAspNetMvcIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Kinoes", "FilmCover_Id", c => c.Int());
            CreateIndex("dbo.Kinoes", "FilmCover_Id");
            AddForeignKey("dbo.Kinoes", "FilmCover_Id", "dbo.FilmCovers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Kinoes", "FilmCover_Id", "dbo.FilmCovers");
            DropIndex("dbo.Kinoes", new[] { "FilmCover_Id" });
            DropColumn("dbo.Kinoes", "FilmCover_Id");
        }
    }
}
