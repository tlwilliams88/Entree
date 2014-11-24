namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateExportSettings : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Configuration.ExportSettings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        ListType = c.Int(),
                        Settings = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Configuration.ExportSettings");
        }
    }
}
