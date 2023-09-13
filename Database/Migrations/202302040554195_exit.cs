namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class exit : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.OrderItems", "Key_Id", "dbo.Keys");
            DropIndex("dbo.OrderItems", new[] { "Key_Id" });
            DropColumn("dbo.OrderItems", "Key_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderItems", "Key_Id", c => c.Int());
            CreateIndex("dbo.OrderItems", "Key_Id");
            AddForeignKey("dbo.OrderItems", "Key_Id", "dbo.Keys", "Id");
        }
    }
}
