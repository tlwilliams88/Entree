namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEnabletoUserPushNotificationDevices : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserPushNotificationDevices", "Enabled", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("Messaging.UserPushNotificationDevices", "Enabled");
        }
    }
}
