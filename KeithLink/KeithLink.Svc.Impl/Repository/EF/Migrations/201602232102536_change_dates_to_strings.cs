namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_dates_to_strings : DbMigration
    {
        public override void Up()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxCustomerNumberByDate");
            AlterColumn("Orders.OrderHistoryHeader", "DeliveryDate", c => c.String(maxLength: 10, fixedLength: true, unicode: false));
            AlterColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTime", c => c.String(maxLength: 19, fixedLength: true, unicode: false));
            AlterColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTime", c => c.String(maxLength: 19, fixedLength: true, unicode: false));
            AlterColumn("Orders.OrderHistoryHeader", "ActualDeliveryTime", c => c.String(maxLength: 19, fixedLength: true, unicode: false));
            CreateIndex("Orders.OrderHistoryHeader", new[] { "CustomerNumber", "DeliveryDate" }, name: "IdxCustomerNumberByDate");
        }
        
        public override void Down()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxCustomerNumberByDate");
            AlterColumn("Orders.OrderHistoryHeader", "ActualDeliveryTime", c => c.DateTime());
            AlterColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTime", c => c.DateTime());
            AlterColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTime", c => c.DateTime());
            AlterColumn("Orders.OrderHistoryHeader", "DeliveryDate", c => c.DateTime());
            CreateIndex("Orders.OrderHistoryHeader", new[] { "CustomerNumber", "DeliveryDate" }, name: "IdxCustomerNumberByDate");
        }
    }
}
