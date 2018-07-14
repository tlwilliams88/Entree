namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalInvoicefields : DbMigration
    {
        public override void Up()
        {
            AddColumn("Invoice.Invoices", "BranchId", c => c.String(maxLength: 3));
            AddColumn("Invoice.Invoices", "InvoiceDate", c => c.DateTime());
            AddColumn("Invoice.Invoices", "Type", c => c.Int(nullable: false));
            AddColumn("Invoice.Invoices", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("Invoice.Invoices", "Status", c => c.Int(nullable: false));
            DropColumn("Invoice.Invoices", "ShipDate");
        }
        
        public override void Down()
        {
            AddColumn("Invoice.Invoices", "ShipDate", c => c.DateTime());
            DropColumn("Invoice.Invoices", "Status");
            DropColumn("Invoice.Invoices", "Amount");
            DropColumn("Invoice.Invoices", "Type");
            DropColumn("Invoice.Invoices", "InvoiceDate");
            DropColumn("Invoice.Invoices", "BranchId");
        }
    }
}
