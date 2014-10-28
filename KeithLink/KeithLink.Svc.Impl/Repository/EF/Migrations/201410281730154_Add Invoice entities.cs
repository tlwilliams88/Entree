namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvoiceentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Invoice.InvoiceItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        LineItem = c.Int(nullable: false),
                        InvoiceType = c.String(),
                        InvoiceDate = c.DateTime(nullable: false),
                        AmountDue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeleteFlag = c.Int(nullable: false),
                        LineNumber = c.Int(nullable: false),
                        QuantityOrdered = c.Int(nullable: false),
                        QuantityShipped = c.Int(nullable: false),
                        BrokenCaseCode = c.String(),
                        CatchWeightCode = c.String(),
                        ExtCatchWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PriceBookNumber = c.String(),
                        ItemPriceSalesRep = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtSalesRepAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtSalesGross = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExtSalesNet = c.Decimal(nullable: false, precision: 18, scale: 2),
                        VendorNumber = c.Int(nullable: false),
                        CustomerPO = c.String(),
                        CombinedStatmentCustomer = c.String(),
                        PriceBook = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                        Invoice_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Invoice.Invoices", t => t.Invoice_Id)
                .Index(t => t.Invoice_Id);
            
            CreateTable(
                "Invoice.Invoices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        InvoiceNumber = c.String(),
                        Type = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false),
                        ShipDate = c.DateTime(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        RouteNumber = c.Int(nullable: false),
                        StopNumber = c.Int(nullable: false),
                        DateTimeOfLastOrder = c.DateTime(nullable: false),
                        CustomerNumber = c.Int(nullable: false),
                        Division = c.String(),
                        Company = c.String(),
                        Department = c.String(),
                        WHNumber = c.String(),
                        OrderNumber = c.Int(nullable: false),
                        MemoBillCode = c.String(),
                        CreditHoldFlag = c.String(),
                        TradeSWFlag = c.String(),
                        CustomerGroup = c.String(),
                        SalesRep = c.String(),
                        ChainStoreCode = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Invoice.InvoiceItems", "Invoice_Id", "Invoice.Invoices");
            DropIndex("Invoice.InvoiceItems", new[] { "Invoice_Id" });
            DropTable("Invoice.Invoices");
            DropTable("Invoice.InvoiceItems");
        }
    }
}
