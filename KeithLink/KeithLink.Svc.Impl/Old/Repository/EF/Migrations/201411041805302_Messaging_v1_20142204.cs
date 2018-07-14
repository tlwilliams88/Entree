namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messaging_v1_20142204 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Messaging.UserMessages",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerNumber = c.String(maxLength: 9, unicode: false),
                        UserId = c.String(maxLength: 55, unicode: false),
                        NotificationType = c.Int(nullable: false),
                        MessageReadUtc = c.DateTime(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Messaging.CustomerTopics",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerNumber = c.String(maxLength: 9, unicode: false),
                        ProviderTopicId = c.String(maxLength: 255, unicode: false),
                        NotificationType = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Messaging.UserTopicSubscriptions",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        ProviderSubscriptionId = c.String(maxLength: 255, unicode: false),
                        NotificationEndpoint = c.String(maxLength: 255, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                        CustomerTopic_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Messaging.CustomerTopics", t => t.CustomerTopic_Id)
                .Index(t => t.CustomerTopic_Id);
            
            CreateTable(
                "Messaging.UserMessagingPreferences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        NotificationType = c.Int(nullable: false),
                        Channel = c.Int(nullable: false),
                        CustomerNumber = c.String(maxLength: 9, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Messaging.UserTopicSubscriptions", "CustomerTopic_Id", "Messaging.CustomerTopics");
            DropIndex("Messaging.UserTopicSubscriptions", new[] { "CustomerTopic_Id" });
            DropTable("Messaging.UserMessagingPreferences");
            DropTable("Messaging.UserTopicSubscriptions");
            DropTable("Messaging.CustomerTopics");
            DropTable("Messaging.UserMessages");
        }
    }
}
