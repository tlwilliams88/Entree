namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SaveLastExportFormat : DbMigration
    {
        public override void Up()
        {
            AddColumn("Configuration.ExportSettings", "ExportFormat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Configuration.ExportSettings", "ExportFormat");
        }
    }
}
