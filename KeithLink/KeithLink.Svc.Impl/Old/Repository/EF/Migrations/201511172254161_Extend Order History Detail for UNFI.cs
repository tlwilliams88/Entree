namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExtendOrderHistoryDetailforUNFI : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.OrderHistoryDetail", "Source", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryDetail", "ManufacturerId", c => c.String(maxLength: 25));
            AddColumn("Orders.OrderHistoryDetail", "SpecialOrderHeaderId", c => c.String(maxLength: 7, fixedLength: true, unicode: false));
            AddColumn("Orders.OrderHistoryDetail", "SpecialOrderLineNumber", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Orders.OrderHistoryDetail", "SpecialOrderLineNumber");
            DropColumn("Orders.OrderHistoryDetail", "SpecialOrderHeaderId");
            DropColumn("Orders.OrderHistoryDetail", "ManufacturerId");
            DropColumn("Orders.OrderHistoryDetail", "Source");
        }
    }
}
