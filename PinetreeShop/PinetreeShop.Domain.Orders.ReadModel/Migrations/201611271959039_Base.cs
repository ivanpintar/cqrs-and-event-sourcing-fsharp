namespace PinetreeShop.Domain.Orders.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Base : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        State = c.String(),
                        StreetAndNumber = c.String(),
                        ZipAndCity = c.String(),
                        StateOrProvince = c.String(),
                        Country = c.String(),
                        LastEventNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Line",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ProductId = c.Guid(nullable: false),
                        ProductName = c.String(),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        Order_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Order", t => t.Order_Id, cascadeDelete: true)
                .Index(t => t.Order_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Line", "Order_Id", "dbo.Order");
            DropIndex("dbo.Line", new[] { "Order_Id" });
            DropTable("dbo.Line");
            DropTable("dbo.Order");
        }
    }
}
