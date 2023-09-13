namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class whatsapp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "isWhatsapp", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "isWhatsapp");
        }
    }
}
