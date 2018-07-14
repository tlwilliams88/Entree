namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class increase_order_history_invoice_length : DbMigration
    {
        public override void Up()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxOrderHeader");
            AlterColumn("Orders.OrderHistoryHeader", "InvoiceNumber", c => c.String(maxLength: 10, unicode: false));
            CreateIndex("Orders.OrderHistoryHeader", new[] { "BranchId", "InvoiceNumber" }, name: "IdxOrderHeader");
        }
        
        public override void Down()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxOrderHeader");
            AlterColumn("Orders.OrderHistoryHeader", "InvoiceNumber", c => c.String(maxLength: 8, fixedLength: true, unicode: false));
            CreateIndex("Orders.OrderHistoryHeader", new[] { "BranchId", "InvoiceNumber" }, name: "IdxOrderHeader");
        }
    }
}
