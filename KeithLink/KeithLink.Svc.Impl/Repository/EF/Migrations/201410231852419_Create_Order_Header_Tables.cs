namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Order_Header_Tables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Orders.OrderHistoryHeader",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        OrderSystem = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        BranchId = c.String(maxLength: 3, fixedLength: true, unicode: false),
                        CustomerNumber = c.String(maxLength: 6, fixedLength: true, unicode: false),
                        InvoiceNumber = c.String(maxLength: 8, fixedLength: true, unicode: false),
                        DeliveryDate = c.DateTime(),
                        PONumber = c.String(maxLength: 20),
                        ControlNumber = c.String(maxLength: 7, fixedLength: true, unicode: false),
                        OrderStatus = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        FutureItems = c.Boolean(nullable: false),
                        ErrorStatus = c.Boolean(nullable: false),
                        RouteNumber = c.String(maxLength: 4, fixedLength: false, unicode: false),
                        StropNumber = c.String(maxLength: 3, fixedLength: true, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Orders.OrderHistoryDetail",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ItemNumber = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        LineNumber = c.Int(nullable: false),
                        OrderQuantity = c.Int(nullable: false),
                        ShippedQuantity = c.Int(nullable: false),
                        UnitOfMeasure = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        CatchWeight = c.Boolean(nullable: false),
                        ItemDeleted = c.Boolean(nullable: false),
                        SubbedOriginalItemNumber = c.String(maxLength: 6, fixedLength: true, unicode: false),
                        ReplacedOriginalItemNumber = c.String(maxLength: 6, fixedLength: true, unicode: false),
                        ItemStatus = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        TotalShippedWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                        OrderHistoryHeader_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Orders.OrderHistoryHeader", t => t.OrderHistoryHeader_Id)
                .Index(t => t.OrderHistoryHeader_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Orders.OrderHistoryDetail", "OrderHistoryHeader_Id", "Orders.OrderHistoryHeader");
            DropIndex("Orders.OrderHistoryDetail", new[] { "OrderHistoryHeader_Id" });
            DropTable("Orders.OrderHistoryDetail");
            DropTable("Orders.OrderHistoryHeader");
        }
    }
}
