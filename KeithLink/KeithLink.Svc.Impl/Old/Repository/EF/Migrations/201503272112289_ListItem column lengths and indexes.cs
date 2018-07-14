namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListItemcolumnlengthsandindexes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("List.ListItems", "ItemNumber", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("List.ListItems", "Label", c => c.String(maxLength: 150));
            AlterColumn("List.ListItems", "Note", c => c.String(maxLength: 200));
            AlterColumn("List.ListItems", "Category", c => c.String(maxLength: 40));

			Sql("CREATE NONCLUSTERED INDEX [IX_ItemParent] ON [List].[ListItems] ([ItemNumber],[ParentList_Id],[Each])");
			Sql("CREATE NONCLUSTERED INDEX [IX_ParentId] ON [List].[ListItems] ([ParentList_Id]) INCLUDE ([ItemNumber],[Category],[Position],[Each])");
        }
        
        public override void Down()
        {
            AlterColumn("List.ListItems", "Category", c => c.String());
            AlterColumn("List.ListItems", "Note", c => c.String());
            AlterColumn("List.ListItems", "Label", c => c.String());
            AlterColumn("List.ListItems", "ItemNumber", c => c.String(nullable: false));

			DropIndex("[List].[ListItems]", "IX_ItemParent");
			DropIndex("[List].[ListItems]", "IX_ParentId");
        }
    }
}
