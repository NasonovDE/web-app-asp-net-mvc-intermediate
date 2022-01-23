namespace WebAppAspNetMvcIdentity.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init7 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FilmCoverKinoes",
                c => new
                    {
                        FilmCover_Id = c.Int(nullable: false),
                        Kino_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FilmCover_Id, t.Kino_Id })
                .ForeignKey("dbo.FilmCovers", t => t.FilmCover_Id, cascadeDelete: true)
                .ForeignKey("dbo.Kinoes", t => t.Kino_Id, cascadeDelete: true)
                .Index(t => t.FilmCover_Id)
                .Index(t => t.Kino_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FilmCoverKinoes", "Kino_Id", "dbo.Kinoes");
            DropForeignKey("dbo.FilmCoverKinoes", "FilmCover_Id", "dbo.FilmCovers");
            DropIndex("dbo.FilmCoverKinoes", new[] { "Kino_Id" });
            DropIndex("dbo.FilmCoverKinoes", new[] { "FilmCover_Id" });
            DropTable("dbo.FilmCoverKinoes");
        }
    }
}
