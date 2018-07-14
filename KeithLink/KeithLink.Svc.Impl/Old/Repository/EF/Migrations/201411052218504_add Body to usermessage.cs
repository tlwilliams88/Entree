namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addBodytousermessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "Body", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Messaging.UserMessages", "Body");
        }
    }
}
