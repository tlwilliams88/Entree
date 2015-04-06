namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class increase_order_detail_invoice_length : DbMigration
    {
        public override void Up()
        {
            DropIndex("Orders.OrderHistoryDetail", "IdxOrderDetail");
            AlterColumn("Orders.OrderHistoryDetail", "InvoiceNumber", c => c.String(maxLength: 10, unicode: false));
            CreateIndex("Orders.OrderHistoryDetail", new[] { "BranchId", "InvoiceNumber", "LineNumber" }, name: "IdxOrderDetail");
        }
        
        public override void Down()
        {
            DropIndex("Orders.OrderHistoryDetail", "IdxOrderDetail");
            AlterColumn("Orders.OrderHistoryDetail", "InvoiceNumber", c => c.String(maxLength: 8, fixedLength: true, unicode: false));
            CreateIndex("Orders.OrderHistoryDetail", new[] { "BranchId", "InvoiceNumber", "LineNumber" }, name: "IdxOrderDetail");
        }
    }
}
