namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adduserpushnotificationdevice20141119 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Messaging.UserPushNotificationDevices",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        DeviceId = c.String(nullable: false, maxLength: 255),
                        ProviderToken = c.String(nullable: false, maxLength: 255),
                        ProviderEndpointId = c.String(maxLength: 255),
                        DeviceOS = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("Messaging.UserPushNotificationDevices", new[] { "UserId" });
            DropTable("Messaging.UserPushNotificationDevices");
        }
    }
}
