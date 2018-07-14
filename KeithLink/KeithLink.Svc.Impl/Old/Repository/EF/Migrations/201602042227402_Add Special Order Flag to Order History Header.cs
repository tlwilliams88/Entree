namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSpecialOrderFlagtoOrderHistoryHeader : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.OrderHistoryHeader", "IsSpecialOrder", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Orders.OrderHistoryHeader", "IsSpecialOrder");
        }
    }
}
