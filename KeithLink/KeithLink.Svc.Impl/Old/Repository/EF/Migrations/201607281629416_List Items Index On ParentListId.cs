namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ListItemsIndexOnParentListId : DbMigration
    {
        private const string NAME_INDEX = "ix_ListItems_ParentListId_Include";

        public override void Up()
        {
            Sql(String.Format(@"CREATE INDEX [{0}] ON [List].[ListItems] ( [ParentList_Id] )
                                INCLUDE( [ItemNumber], [Label], [Par], 
                                         [Note], [CreatedUtc], [ModifiedUtc], 
                                         [Category], [Position], [FromDate], 
                                         [ToDate], [Each], [Quantity], 
                                         [CatalogId] )", NAME_INDEX));
        }
        
        public override void Down()
        {
            DropIndex("[List].[ListItems]", NAME_INDEX);
        }
    }
}
