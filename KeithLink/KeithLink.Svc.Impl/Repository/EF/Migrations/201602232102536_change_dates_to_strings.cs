namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_dates_to_strings : DbMigration
    {
        public override void Up()
        {
            DropIndex("Orders.OrderHistoryHeader", "IdxCustomerNumberByDate");
            AddColumn("Orders.OrderHistoryHeader", "DeliveryDateTemp", c => c.String(maxLength: 10, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTimeTemp", c => c.String(maxLength: 19, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTimeTemp", c => c.String(maxLength: 19, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryHeader", "ActualDeliveryTimeTemp", c => c.String(maxLength: 19, fixedLength: true, unicode: false));

            Sql(@"
                UPDATE Orders.OrderHistoryHeader
                SET DeliveryDateTemp = CONVERT(CHAR(10), DeliveryDate, 101),
                    ScheduledDeliveryTimeTemp = CASE
                        WHEN ScheduledDeliveryTime IS NOT NULL THEN
                            CONVERT(CHAR(10), ScheduledDeliveryTime, 101) + ' ' + CONVERT(CHAR(8), ScheduledDeliveryTime, 114)
                        ELSE
                            NULL
                    END,
                    EstimatedDeliveryTimeTemp = CASE
                        WHEN EstimatedDeliveryTime IS NOT NULL THEN
                            CONVERT(CHAR(10), EstimatedDeliveryTime, 101) + ' ' + CONVERT(CHAR(8), EstimatedDeliveryTime, 114)
                        ELSE
                            NULL
                        END,
                    ActualDeliveryTimeTemp = CASE
                        WHEN ActualDeliveryTime IS NOT NULL THEN
                            CONVERT(CHAR(10), ActualDeliveryTime, 101) + ' ' + CONVERT(CHAR(8), ActualDeliveryTime, 114)
                        ELSE
                            NULL
                    END
            ");

            DropColumn("Orders.OrderHistoryHeader", "DeliveryDate");
            DropColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTime");
            DropColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTime");
            DropColumn("Orders.OrderHistoryHeader", "ActualDeliveryTime");

            RenameColumn("Orders.OrderHistoryHeader", "DeliveryDateTemp", "DeliveryDate");
            RenameColumn("Orders.OrderHistoryHeader", "ScheduledDeliveryTimeTemp", "ScheduledDeliveryTime");
            RenameColumn("Orders.OrderHistoryHeader", "EstimatedDeliveryTimeTemp", "EstimatedDeliveryTime");
            RenameColumn("Orders.OrderHistoryHeader", "ActualDeliveryTimeTemp", "ActualDeliveryTime");

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
