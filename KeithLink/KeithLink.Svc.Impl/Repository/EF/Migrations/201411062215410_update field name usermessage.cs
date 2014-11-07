namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefieldnameusermessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "Subject", c => c.String());
            DropColumn("Messaging.UserMessages", "Message");
        }
        
        public override void Down()
        {
            AddColumn("Messaging.UserMessages", "Message", c => c.String());
            DropColumn("Messaging.UserMessages", "Subject");
        }
    }
}
