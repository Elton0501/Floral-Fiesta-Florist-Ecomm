namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seasonal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Seasonal", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Seasonal");
        }
    }
}
