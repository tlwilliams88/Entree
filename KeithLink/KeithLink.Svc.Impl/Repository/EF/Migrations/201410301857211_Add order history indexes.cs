namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addorderhistoryindexes : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.OrderHistoryDetail", "BranchId", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryDetail", "InvoiceNumber", c => c.String(maxLength: 8, fixedLength: true, unicode: false));
            CreateIndex("Orders.OrderHistoryDetail", new[] { "BranchId", "InvoiceNumber", "LineNumber" }, name: "IdxOrderDetail");
            CreateIndex("Orders.OrderHistoryHeader", new[] { "BranchId", "InvoiceNumber" }, name: "IdxOrderHeader");
        }
        
        public override void Down()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxOrderHeader");
            DropIndex("Orders.OrderHistoryDetail", "IdxOrderDetail");
            DropColumn("Orders.OrderHistoryDetail", "InvoiceNumber");
            DropColumn("Orders.OrderHistoryDetail", "BranchId");
        }
    }
}
