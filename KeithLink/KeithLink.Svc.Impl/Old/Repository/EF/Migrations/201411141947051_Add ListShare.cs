namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddListShare : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "List.ListShares",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CustomerId = c.String(),
                        BranchId = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                        SharedList_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("List.Lists", t => t.SharedList_Id)
                .Index(t => t.SharedList_Id);
            
            DropColumn("List.Lists", "Shared");
        }
        
        public override void Down()
        {
            AddColumn("List.Lists", "Shared", c => c.Boolean(nullable: false));
            DropForeignKey("List.ListShares", "SharedList_Id", "List.Lists");
            DropIndex("List.ListShares", new[] { "SharedList_Id" });
            DropTable("List.ListShares");
        }
    }
}
