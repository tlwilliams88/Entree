namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddorderETAfieldstoorderhistoryheader : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.OrderHistoryHeader", "StopNumber", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTime", c => c.DateTime());
            AddColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTime", c => c.DateTime());
            AddColumn("Orders.OrderHistoryHeader", "ActualDeliveryTime", c => c.DateTime());
            AddColumn("Orders.OrderHistoryHeader", "DeliveryOutOfSequence", c => c.Boolean());
            DropColumn("Orders.OrderHistoryHeader", "StropNumber");
        }
        
        public override void Down()
        {
            AddColumn("Orders.OrderHistoryHeader", "StropNumber", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
            DropColumn("Orders.OrderHistoryHeader", "DeliveryOutOfSequence");
            DropColumn("Orders.OrderHistoryHeader", "ActualDeliveryTime");
            DropColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTime");
            DropColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTime");
            DropColumn("Orders.OrderHistoryHeader", "StopNumber");
        }
    }
}
