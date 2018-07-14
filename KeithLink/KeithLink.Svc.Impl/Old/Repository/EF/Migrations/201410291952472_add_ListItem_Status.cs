namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_ListItem_Status : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("List.ListItems", "Status");
        }
    }
}
