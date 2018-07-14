namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _201502032124119_20150203_change_route_number_maxlength_to_four : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Orders.OrderHistoryHeader", "RouteNumber", c => c.String(maxLength: 4, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("Orders.OrderHistoryHeader", "RouteNumber", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
        }
    }
}
