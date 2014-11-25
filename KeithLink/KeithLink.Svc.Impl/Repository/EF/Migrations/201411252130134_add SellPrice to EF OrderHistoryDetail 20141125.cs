namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addSellPricetoEFOrderHistoryDetail20141125 : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.OrderHistoryDetail", "SellPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("Orders.OrderHistoryDetail", "SellPrice");
        }
    }
}
