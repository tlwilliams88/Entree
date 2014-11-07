namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeduseridtoguidusermessage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Messaging.UserMessages", "UserId", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("Messaging.UserMessages", "UserId", c => c.String(maxLength: 55, unicode: false));
        }
    }
}
