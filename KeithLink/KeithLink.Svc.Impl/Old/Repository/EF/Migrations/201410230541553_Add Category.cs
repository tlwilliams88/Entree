namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "Category", c => c.String());
            DropColumn("List.ListItems", "Each");
        }
        
        public override void Down()
        {
            AddColumn("List.ListItems", "Each", c => c.Boolean(nullable: false));
            DropColumn("List.ListItems", "Category");
        }
    }
}
