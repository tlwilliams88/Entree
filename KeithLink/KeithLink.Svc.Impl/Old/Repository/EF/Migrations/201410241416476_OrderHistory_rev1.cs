namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderHistory_rev1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Orders.OrderHistoryDetail", "ItemNumber", c => c.String(maxLength: 6, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("Orders.OrderHistoryDetail", "ItemNumber", c => c.String(maxLength: 1, fixedLength: true, unicode: false));
        }
    }
}
