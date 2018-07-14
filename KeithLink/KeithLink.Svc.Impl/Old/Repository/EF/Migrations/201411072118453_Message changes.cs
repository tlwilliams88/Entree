namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messagechanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "OrderNumber", c => c.String());
            AddColumn("Messaging.UserMessages", "Subject", c => c.String());
            AddColumn("Messaging.UserMessages", "Mandatory", c => c.Boolean(nullable: false));
            AlterColumn("Messaging.UserMessages", "UserId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Messaging.UserMessages", "UserId", c => c.String(maxLength: 55, unicode: false));
            DropColumn("Messaging.UserMessages", "Mandatory");
            DropColumn("Messaging.UserMessages", "Subject");
            DropColumn("Messaging.UserMessages", "OrderNumber");
        }
    }
}
