namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_branch_to_messaging_prefs : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessagingPreferences", "BranchId", c => c.String(maxLength: 4, unicode: false));
        }
        
        public override void Down()
        {
            DropColumn("Messaging.UserMessagingPreferences", "BranchId");
        }
    }
}
