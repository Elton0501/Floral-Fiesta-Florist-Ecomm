namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class descriptions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderItems", "IDesc", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderItems", "IDesc");
        }
    }
}
