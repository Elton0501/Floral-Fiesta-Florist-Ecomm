namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "DDate", c => c.String());
            AddColumn("dbo.Carts", "DSlot", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "DSlot");
            DropColumn("dbo.Carts", "DDate");
        }
    }
}
