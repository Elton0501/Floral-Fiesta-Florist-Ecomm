namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mobilenoremove : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Users", "MobileNo");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "MobileNo", c => c.String());
        }
    }
}
