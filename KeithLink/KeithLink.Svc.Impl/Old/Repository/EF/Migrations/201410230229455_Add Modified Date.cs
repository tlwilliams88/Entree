namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModifiedDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "ModifiedUtc", c => c.DateTime(nullable: false));
            AddColumn("List.Lists", "ModifiedUtc", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("List.Lists", "ModifiedUtc");
            DropColumn("List.ListItems", "ModifiedUtc");
        }
    }
}
