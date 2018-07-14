namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cmsaddproductid20141117 : DbMigration
    {
        public override void Up()
        {
            AddColumn("ContentManagement.ContentItems", "ProductId", c => c.String(maxLength: 24));
        }
        
        public override void Down()
        {
            DropColumn("ContentManagement.ContentItems", "ProductId");
        }
    }
}
