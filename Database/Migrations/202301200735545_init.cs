namespace Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        HouseNo = c.String(),
                        Street = c.String(),
                        City = c.String(),
                        LocalArea = c.String(),
                        LandMark = c.String(),
                        Pincode = c.String(),
                        Country = c.String(),
                        State = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.Boolean(nullable: false),
                        BannerImage = c.String(),
                        CoverImage = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SubCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Status = c.Boolean(nullable: false),
                        Create = c.DateTime(nullable: false),
                        CategoryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Isfeatured = c.Boolean(nullable: false),
                        Status = c.Boolean(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        SubCategoryId = c.Int(nullable: false),
                        SeoTags = c.String(nullable: false),
                        Rating = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        imgdefault = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SubCategories", t => t.SubCategoryId, cascadeDelete: true)
                .Index(t => t.SubCategoryId);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.String(),
                        Create = c.DateTime(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Default = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        Message = c.String(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Keys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Type = c.String(),
                        Description = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Newsletters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Status = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        isMultipleUser = c.Boolean(nullable: false),
                        TypeId = c.String(),
                        AnchorLink = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        createdBy = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        razorpayKey = c.String(),
                        currency = c.String(),
                        description = c.String(),
                        rzp_orderid = c.String(),
                        ReceiverName = c.String(nullable: false),
                        ReceiverMobileNumber = c.String(nullable: false),
                        ReceiverEmail = c.String(nullable: false),
                        ShippingAddress = c.String(nullable: false),
                        PinCode = c.String(nullable: false),
                        City = c.String(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        PaymentStatus = c.Int(nullable: false),
                        OrderStatus = c.Int(nullable: false),
                        BillAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryCharges = c.Int(nullable: false),
                        DeliveredDate = c.String(),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.OrderId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Guid(nullable: false),
                        RoleName = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MobileNumber = c.String(),
                        Name = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        Varified = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "UserId", "dbo.Users");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Products", "SubCategoryId", "dbo.SubCategories");
            DropForeignKey("dbo.ProductImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.SubCategories", "CategoryId", "dbo.Categories");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.ProductImages", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "SubCategoryId" });
            DropIndex("dbo.SubCategories", new[] { "CategoryId" });
            DropIndex("dbo.Addresses", new[] { "UserId" });
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Notifications");
            DropTable("dbo.Newsletters");
            DropTable("dbo.Keys");
            DropTable("dbo.Contacts");
            DropTable("dbo.ProductImages");
            DropTable("dbo.Products");
            DropTable("dbo.SubCategories");
            DropTable("dbo.Categories");
            DropTable("dbo.Carts");
            DropTable("dbo.Addresses");
        }
    }
}
