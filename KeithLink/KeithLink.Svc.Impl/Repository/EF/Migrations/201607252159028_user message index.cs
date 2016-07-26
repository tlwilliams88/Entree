namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class usermessageindex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Messaging.UserMessages", new[] { "UserId", "MessageReadUtc" }, name: "idx_UserId_ReadDateUtc");
        }
        
        public override void Down()
        {
            DropIndex("Messaging.UserMessages", "idx_UserId_ReadDateUtc");
        }
    }
}
