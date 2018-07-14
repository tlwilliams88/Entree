namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInvoiceDueDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("Invoice.Invoices", "DueDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("Invoice.Invoices", "DueDate");
        }
    }
}
