namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class slots : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "SlotId", c => c.String());
            AddColumn("dbo.OrderItems", "DDate", c => c.String());
            AddColumn("dbo.OrderItems", "Key_Id", c => c.Int());
            CreateIndex("dbo.OrderItems", "Key_Id");
            AddForeignKey("dbo.OrderItems", "Key_Id", "dbo.Keys", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "Key_Id", "dbo.Keys");
            DropIndex("dbo.OrderItems", new[] { "Key_Id" });
            DropColumn("dbo.OrderItems", "Key_Id");
            DropColumn("dbo.OrderItems", "DDate");
            DropColumn("dbo.OrderItems", "SlotId");
        }
    }
}
