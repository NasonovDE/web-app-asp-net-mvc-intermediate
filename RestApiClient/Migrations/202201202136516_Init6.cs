namespace WebAppAspNetMvcIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Kinoes", "FilmCover_Id", "dbo.FilmCovers");
            DropIndex("dbo.Kinoes", new[] { "FilmCover_Id" });
            DropColumn("dbo.Kinoes", "FilmCover_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Kinoes", "FilmCover_Id", c => c.Int());
            CreateIndex("dbo.Kinoes", "FilmCover_Id");
            AddForeignKey("dbo.Kinoes", "FilmCover_Id", "dbo.FilmCovers", "Id");
        }
    }
}
