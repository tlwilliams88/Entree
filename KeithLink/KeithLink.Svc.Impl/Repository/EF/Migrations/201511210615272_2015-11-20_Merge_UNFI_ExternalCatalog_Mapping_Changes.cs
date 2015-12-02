namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20151120_Merge_UNFI_ExternalCatalog_Mapping_Changes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Configuration.ExternalCatalogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BekBranchId = c.String(maxLength: 24),
                        ExternalBranchId = c.String(maxLength: 24),
                        Type = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Profile.Settings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        Key = c.String(nullable: false, maxLength: 100, unicode: false),
                        Value = c.String(nullable: false, maxLength: 250, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserId);
            
            AddColumn("List.ListItems", "CatalogId", c => c.String(maxLength: 24));
            AddColumn("Orders.OrderHistoryHeader", "OriginalControlNumber", c => c.String(maxLength: 7, fixedLength: true, unicode: false));
        }
        
        public override void Down()
        {
            DropIndex("Profile.Settings", new[] { "UserId" });
            DropColumn("Orders.OrderHistoryHeader", "OriginalControlNumber");
            DropColumn("List.ListItems", "CatalogId");
            DropTable("Profile.Settings");
            DropTable("Configuration.ExternalCatalogs");
        }
    }
}
