﻿namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testemonial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Testemonials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Testimonial = c.String(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Testemonials");
        }
    }
}
