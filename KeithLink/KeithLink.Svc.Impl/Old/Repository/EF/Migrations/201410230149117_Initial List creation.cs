namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialListcreation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "List.ListItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        ItemNumber = c.String(),
                        Label = c.String(),
                        Par = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Each = c.Boolean(nullable: false),
                        Note = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        List_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("List.Lists", t => t.List_Id)
                .Index(t => t.List_Id);
            
            CreateTable(
                "List.Lists",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        DisplayName = c.String(),
                        Type = c.Int(nullable: false),
                        CustomerId = c.String(),
                        BranchId = c.String(),
                        AccountNumber = c.String(),
                        Shared = c.Boolean(nullable: false),
                        ReadOnly = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("List.ListItems", "List_Id", "List.Lists");
            DropIndex("List.ListItems", new[] { "List_Id" });
            DropTable("List.Lists");
            DropTable("List.ListItems");
        }
    }
}
