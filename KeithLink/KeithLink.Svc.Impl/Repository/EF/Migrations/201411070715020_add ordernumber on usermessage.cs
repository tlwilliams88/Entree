namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addordernumberonusermessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "OrderNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Messaging.UserMessages", "OrderNumber");
        }
    }
}
