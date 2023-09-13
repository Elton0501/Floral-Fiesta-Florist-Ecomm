namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mobileno : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "MobileNo", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "MobileNo");
        }
    }
}
