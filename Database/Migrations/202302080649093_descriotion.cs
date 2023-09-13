namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class descriotion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Carts", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Carts", "Description");
        }
    }
}
