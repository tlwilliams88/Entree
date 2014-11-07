namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fieldsaddedtousermessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "MessageCreatedUtc", c => c.DateTime());
            AddColumn("Messaging.UserMessages", "Message", c => c.String());
            AddColumn("Messaging.UserMessages", "Mandatory", c => c.Boolean(nullable: false));
            AddColumn("Messaging.UserTopicSubscriptions", "Channel", c => c.Int(nullable: false));
            DropColumn("Messaging.UserTopicSubscriptions", "NotificationType");
        }
        
        public override void Down()
        {
            AddColumn("Messaging.UserTopicSubscriptions", "NotificationType", c => c.Int(nullable: false));
            DropColumn("Messaging.UserTopicSubscriptions", "Channel");
            DropColumn("Messaging.UserMessages", "Mandatory");
            DropColumn("Messaging.UserMessages", "Message");
            DropColumn("Messaging.UserMessages", "MessageCreatedUtc");
        }
    }
}
