namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Channel_To_User_Subscription : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserTopicSubscriptions", "Channel", c => c.Int(nullable: false));
            DropColumn("Messaging.UserTopicSubscriptions", "NotificationType");
        }
        
        public override void Down()
        {
            AddColumn("Messaging.UserTopicSubscriptions", "NotificationType", c => c.Int(nullable: false));
            DropColumn("Messaging.UserTopicSubscriptions", "Channel");
        }
    }
}
