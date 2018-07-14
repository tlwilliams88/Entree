namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Reworkinvoiceentities : DbMigration
    {
        public override void Up()
        {
            AddColumn("Invoice.InvoiceItems", "ClassCode", c => c.String(maxLength: 2, fixedLength: true, unicode: false));
            AlterColumn("Invoice.InvoiceItems", "CatchWeightCode", c => c.Boolean(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "ItemNumber", c => c.String(maxLength: 6, fixedLength: true, unicode: false));
            AlterColumn("Invoice.Invoices", "CustomerNumber", c => c.String(maxLength: 10, fixedLength: true, unicode: false));
            DropColumn("Invoice.InvoiceItems", "LineItem");
            DropColumn("Invoice.InvoiceItems", "InvoiceType");
            DropColumn("Invoice.InvoiceItems", "InvoiceDate");
            DropColumn("Invoice.InvoiceItems", "AmountDue");
            DropColumn("Invoice.InvoiceItems", "DeleteFlag");
            DropColumn("Invoice.InvoiceItems", "BrokenCaseCode");
            DropColumn("Invoice.InvoiceItems", "PriceBookNumber");
            DropColumn("Invoice.InvoiceItems", "ItemPriceSalesRep");
            DropColumn("Invoice.InvoiceItems", "ExtSalesRepAmount");
            DropColumn("Invoice.InvoiceItems", "ExtSalesGross");
            DropColumn("Invoice.InvoiceItems", "VendorNumber");
            DropColumn("Invoice.InvoiceItems", "CustomerPO");
            DropColumn("Invoice.InvoiceItems", "CombinedStatmentCustomer");
            DropColumn("Invoice.InvoiceItems", "PriceBook");
            DropColumn("Invoice.Invoices", "Type");
            DropColumn("Invoice.Invoices", "DueDate");
            DropColumn("Invoice.Invoices", "RouteNumber");
            DropColumn("Invoice.Invoices", "StopNumber");
            DropColumn("Invoice.Invoices", "DateTimeOfLastOrder");
            DropColumn("Invoice.Invoices", "Division");
            DropColumn("Invoice.Invoices", "Company");
            DropColumn("Invoice.Invoices", "Department");
            DropColumn("Invoice.Invoices", "WHNumber");
            DropColumn("Invoice.Invoices", "OrderNumber");
            DropColumn("Invoice.Invoices", "MemoBillCode");
            DropColumn("Invoice.Invoices", "CreditHoldFlag");
            DropColumn("Invoice.Invoices", "TradeSWFlag");
            DropColumn("Invoice.Invoices", "CustomerGroup");
            DropColumn("Invoice.Invoices", "SalesRep");
            DropColumn("Invoice.Invoices", "ChainStoreCode");
        }
        
        public override void Down()
        {
            AddColumn("Invoice.Invoices", "ChainStoreCode", c => c.String());
            AddColumn("Invoice.Invoices", "SalesRep", c => c.String());
            AddColumn("Invoice.Invoices", "CustomerGroup", c => c.String());
            AddColumn("Invoice.Invoices", "TradeSWFlag", c => c.String());
            AddColumn("Invoice.Invoices", "CreditHoldFlag", c => c.String());
            AddColumn("Invoice.Invoices", "MemoBillCode", c => c.String());
            AddColumn("Invoice.Invoices", "OrderNumber", c => c.Int());
            AddColumn("Invoice.Invoices", "WHNumber", c => c.String());
            AddColumn("Invoice.Invoices", "Department", c => c.String());
            AddColumn("Invoice.Invoices", "Company", c => c.String());
            AddColumn("Invoice.Invoices", "Division", c => c.String());
            AddColumn("Invoice.Invoices", "DateTimeOfLastOrder", c => c.DateTime());
            AddColumn("Invoice.Invoices", "StopNumber", c => c.Int());
            AddColumn("Invoice.Invoices", "RouteNumber", c => c.Int());
            AddColumn("Invoice.Invoices", "DueDate", c => c.DateTime());
            AddColumn("Invoice.Invoices", "Type", c => c.Int(nullable: false));
            AddColumn("Invoice.InvoiceItems", "PriceBook", c => c.String());
            AddColumn("Invoice.InvoiceItems", "CombinedStatmentCustomer", c => c.String());
            AddColumn("Invoice.InvoiceItems", "CustomerPO", c => c.String());
            AddColumn("Invoice.InvoiceItems", "VendorNumber", c => c.Int());
            AddColumn("Invoice.InvoiceItems", "ExtSalesGross", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Invoice.InvoiceItems", "ExtSalesRepAmount", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Invoice.InvoiceItems", "ItemPriceSalesRep", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Invoice.InvoiceItems", "PriceBookNumber", c => c.String());
            AddColumn("Invoice.InvoiceItems", "BrokenCaseCode", c => c.String());
            AddColumn("Invoice.InvoiceItems", "DeleteFlag", c => c.Int());
            AddColumn("Invoice.InvoiceItems", "AmountDue", c => c.Decimal(precision: 18, scale: 2));
            AddColumn("Invoice.InvoiceItems", "InvoiceDate", c => c.DateTime());
            AddColumn("Invoice.InvoiceItems", "InvoiceType", c => c.String());
            AddColumn("Invoice.InvoiceItems", "LineItem", c => c.Int(nullable: false));
            AlterColumn("Invoice.Invoices", "CustomerNumber", c => c.Int(nullable: false));
            AlterColumn("Invoice.InvoiceItems", "ItemNumber", c => c.String());
            AlterColumn("Invoice.InvoiceItems", "CatchWeightCode", c => c.String());
            DropColumn("Invoice.InvoiceItems", "ClassCode");
        }
    }
}
