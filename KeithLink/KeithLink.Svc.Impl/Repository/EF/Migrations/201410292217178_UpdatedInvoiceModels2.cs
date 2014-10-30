namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedInvoiceModels2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("Invoice.InvoiceItems", "LineNumber");
        }
        
        public override void Down()
        {
            AddColumn("Invoice.InvoiceItems", "LineNumber", c => c.Int());
        }
    }
}
