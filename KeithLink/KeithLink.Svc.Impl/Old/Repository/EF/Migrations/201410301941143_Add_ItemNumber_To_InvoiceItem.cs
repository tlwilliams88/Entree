namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ItemNumber_To_InvoiceItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("Invoice.InvoiceItems", "ItemNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Invoice.InvoiceItems", "ItemNumber");
        }
    }
}
