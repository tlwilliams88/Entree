namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_each_to_listitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "Each", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("List.ListItems", "Each");
        }
    }
}
