namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20151120_AddCatalogIdToListItemToSupportUnfi : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "CatalogId", c => c.String(maxLength: 24));
        }
        
        public override void Down()
        {
            DropColumn("List.ListItems", "CatalogId");
        }
    }
}
