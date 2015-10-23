namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20151023addingexternalcatalog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Configuration.ExternalCatalogs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BekBranchId = c.String(),
                        ExternalBranchId = c.String(),
                        Type = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Configuration.ExternalCatalogs");
        }
    }
}
