namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinktoinvoicefromInvoiceItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Invoice.InvoiceItems", "Invoice_Id", "Invoice.Invoices");
            DropIndex("Invoice.InvoiceItems", new[] { "Invoice_Id" });
            RenameColumn(table: "Invoice.InvoiceItems", name: "Invoice_Id", newName: "InvoiceId");
            AlterColumn("Invoice.InvoiceItems", "InvoiceId", c => c.Long(nullable: false));
            CreateIndex("Invoice.InvoiceItems", "InvoiceId");
            AddForeignKey("Invoice.InvoiceItems", "InvoiceId", "Invoice.Invoices", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("Invoice.InvoiceItems", "InvoiceId", "Invoice.Invoices");
            DropIndex("Invoice.InvoiceItems", new[] { "InvoiceId" });
            AlterColumn("Invoice.InvoiceItems", "InvoiceId", c => c.Long());
            RenameColumn(table: "Invoice.InvoiceItems", name: "InvoiceId", newName: "Invoice_Id");
            CreateIndex("Invoice.InvoiceItems", "Invoice_Id");
            AddForeignKey("Invoice.InvoiceItems", "Invoice_Id", "Invoice.Invoices", "Id");
        }
    }
}
