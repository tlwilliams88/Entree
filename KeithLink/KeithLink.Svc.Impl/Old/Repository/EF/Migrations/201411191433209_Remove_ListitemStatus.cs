namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_ListitemStatus : DbMigration
    {
        public override void Up()
        {
            DropColumn("List.ListItems", "Status");
        }
        
        public override void Down()
        {
            AddColumn("List.ListItems", "Status", c => c.Int(nullable: false));
        }
    }
}
