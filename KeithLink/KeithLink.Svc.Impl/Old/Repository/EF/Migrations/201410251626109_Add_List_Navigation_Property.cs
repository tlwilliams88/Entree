namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_List_Navigation_Property : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "List.ListItems", name: "List_Id", newName: "ParentList_Id");
            RenameIndex(table: "List.ListItems", name: "IX_List_Id", newName: "IX_ParentList_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "List.ListItems", name: "IX_ParentList_Id", newName: "IX_List_Id");
            RenameColumn(table: "List.ListItems", name: "ParentList_Id", newName: "List_Id");
        }
    }
}
