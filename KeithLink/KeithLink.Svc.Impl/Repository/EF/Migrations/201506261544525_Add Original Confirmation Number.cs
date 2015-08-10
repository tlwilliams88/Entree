namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOriginalConfirmationNumber : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.OrderHistoryHeader", "OriginalControlNumber", c => c.String(maxLength: 7, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Orders.OrderHistoryHeader", "OriginalControlNumber");
        }
    }
}
