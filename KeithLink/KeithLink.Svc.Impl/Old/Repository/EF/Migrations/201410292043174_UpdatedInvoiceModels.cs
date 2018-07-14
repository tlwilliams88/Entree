namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedInvoiceModels : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Invoice.InvoiceItems", "InvoiceDate", c => c.DateTime());
            AlterColumn("Invoice.InvoiceItems", "AmountDue", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "DeleteFlag", c => c.Int());
            AlterColumn("Invoice.InvoiceItems", "LineNumber", c => c.Int());
            AlterColumn("Invoice.InvoiceItems", "QuantityOrdered", c => c.Int());
            AlterColumn("Invoice.InvoiceItems", "QuantityShipped", c => c.Int());
            AlterColumn("Invoice.InvoiceItems", "ExtCatchWeight", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ItemPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ItemPriceSalesRep", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ExtSalesRepAmount", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ExtSalesGross", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ExtSalesNet", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "VendorNumber", c => c.Int());
            AlterColumn("Invoice.Invoices", "DueDate", c => c.DateTime());
            AlterColumn("Invoice.Invoices", "ShipDate", c => c.DateTime());
            AlterColumn("Invoice.Invoices", "OrderDate", c => c.DateTime());
            AlterColumn("Invoice.Invoices", "RouteNumber", c => c.Int());
            AlterColumn("Invoice.Invoices", "StopNumber", c => c.Int());
            AlterColumn("Invoice.Invoices", "DateTimeOfLastOrder", c => c.DateTime());
            AlterColumn("Invoice.Invoices", "OrderNumber", c => c.Int());
            AlterColumn("Invoice.Invoices", "ChainStoreCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Invoice.Invoices", "ChainStoreCode", c => c.Int(nullable: false));
            AlterColumn("Invoice.Invoices", "OrderNumber", c => c.Int(nullable: false));
            AlterColumn("Invoice.Invoices", "DateTimeOfLastOrder", c => c.DateTime(nullable: false));
            AlterColumn("Invoice.Invoices", "StopNumber", c => c.Int(nullable: false));
            AlterColumn("Invoice.Invoices", "RouteNumber", c => c.Int(nullable: false));
            AlterColumn("Invoice.Invoices", "OrderDate", c => c.DateTime(nullable: false));
            AlterColumn("Invoice.Invoices", "ShipDate", c => c.DateTime(nullable: false));
            AlterColumn("Invoice.Invoices", "DueDate", c => c.DateTime(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "VendorNumber", c => c.Int(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "ExtSalesNet", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ExtSalesGross", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ExtSalesRepAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ItemPriceSalesRep", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ItemPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "ExtCatchWeight", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "QuantityShipped", c => c.Int(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "QuantityOrdered", c => c.Int(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "LineNumber", c => c.Int(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "DeleteFlag", c => c.Int(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "AmountDue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("Invoice.InvoiceItems", "InvoiceDate", c => c.DateTime(nullable: false));
        }
    }
}
