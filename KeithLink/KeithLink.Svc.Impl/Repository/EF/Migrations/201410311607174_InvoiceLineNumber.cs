namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InvoiceLineNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("Invoice.InvoiceItems", "LineNumber", c => c.String(maxLength: 6));
            AlterColumn("Invoice.InvoiceItems", "ItemNumber", c => c.String(maxLength: 10));
        }
        
        public override void Down()
        {
            AlterColumn("Invoice.InvoiceItems", "ItemNumber", c => c.String(maxLength: 6, fixedLength: true, unicode: false));
            DropColumn("Invoice.InvoiceItems", "LineNumber");
        }
    }
}
