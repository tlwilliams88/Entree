namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_List_Index : DbMigration
    {
        public override void Up()
        {
            AlterColumn("List.Lists", "CustomerId", c => c.String(maxLength: 10));
            AlterColumn("List.Lists", "BranchId", c => c.String(maxLength: 10));
            CreateIndex("List.Lists", "UserId");
            CreateIndex("List.Lists", "Type");
            CreateIndex("List.Lists", new[] { "CustomerId", "BranchId" }, name: "IX_CustBranch");
        }
        
        public override void Down()
        {
            DropIndex("List.Lists", "IX_CustBranch");
            DropIndex("List.Lists", new[] { "Type" });
            DropIndex("List.Lists", new[] { "UserId" });
            AlterColumn("List.Lists", "BranchId", c => c.String());
            AlterColumn("List.Lists", "CustomerId", c => c.String());
        }
    }
}
