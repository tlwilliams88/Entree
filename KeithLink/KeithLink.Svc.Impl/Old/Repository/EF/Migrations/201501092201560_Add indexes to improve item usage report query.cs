namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addindexestoimproveitemusagereportquery : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Orders.OrderHistoryDetail", new[] { "ItemNumber", "UnitOfMeasure" }, name: "IdxItemUsageGrouping");
            CreateIndex("Orders.OrderHistoryHeader", new[] { "CustomerNumber", "DeliveryDate" }, name: "IdxCustomerNumberByDate");
        }
        
        public override void Down()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxCustomerNumberByDate");
            DropIndex("Orders.OrderHistoryDetail", "IdxItemUsageGrouping");
        }
    }
}
