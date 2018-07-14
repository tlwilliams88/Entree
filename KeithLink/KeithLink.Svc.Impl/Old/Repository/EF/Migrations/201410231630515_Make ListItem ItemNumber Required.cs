namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeListItemItemNumberRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("List.ListItems", "ItemNumber", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("List.ListItems", "ItemNumber", c => c.String());
        }
    }
}
