namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedusermessagelabel : DbMigration
    {
        public override void Up()
        {
            AddColumn("Messaging.UserMessages", "Label", c => c.String());
            DropColumn("Messaging.UserMessages", "OrderNumber");
        }
        
        public override void Down()
        {
            AddColumn("Messaging.UserMessages", "OrderNumber", c => c.String());
            DropColumn("Messaging.UserMessages", "Label");
        }
    }
}
