namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20151120_ShortenExternalCatalogFieldNames : DbMigration
    {
        public override void Up()
        {
            AlterColumn("Configuration.ExternalCatalogs", "BekBranchId", c => c.String(maxLength: 24));
            AlterColumn("Configuration.ExternalCatalogs", "ExternalBranchId", c => c.String(maxLength: 24));
        }
        
        public override void Down()
        {
            AlterColumn("Configuration.ExternalCatalogs", "ExternalBranchId", c => c.String());
            AlterColumn("Configuration.ExternalCatalogs", "BekBranchId", c => c.String());
        }
    }
}
