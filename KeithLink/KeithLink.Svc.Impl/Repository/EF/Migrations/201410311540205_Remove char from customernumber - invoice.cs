namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Removecharfromcustomernumberinvoice : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Invoice.Invoices", "CustomerNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("Invoice.Invoices", "CustomerNumber", c => c.String(maxLength: 10, fixedLength: true, unicode: false));
        }
    }
}
