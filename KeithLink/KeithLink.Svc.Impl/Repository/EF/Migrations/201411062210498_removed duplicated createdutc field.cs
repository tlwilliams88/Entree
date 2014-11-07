namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removedduplicatedcreatedutcfield : DbMigration
    {
        public override void Up()
        {
            DropColumn("Messaging.UserMessages", "MessageCreatedUtc");
        }
        
        public override void Down()
        {
            AddColumn("Messaging.UserMessages", "MessageCreatedUtc", c => c.DateTime());
        }
    }
}
