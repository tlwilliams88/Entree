namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_List_UserID_Index : DbMigration
    {
        public override void Up()
        {
            DropIndex("List.Lists", new[] { "UserId" });
        }
        
        public override void Down()
        {
            CreateIndex("List.Lists", "UserId");
        }
    }
}
