namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_CustomerName_Branch_To_UserMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "CustomerName", c => c.String(maxLength: 250));
            AddColumn("Messaging.UserMessages", "BranchId", c => c.String(maxLength: 3));
        }
        
        public override void Down()
        {
            DropColumn("Messaging.UserMessages", "BranchId");
            DropColumn("Messaging.UserMessages", "CustomerName");
        }
    }
}
