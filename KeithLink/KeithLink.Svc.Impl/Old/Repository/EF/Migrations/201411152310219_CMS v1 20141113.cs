namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CMSv120141113 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "ContentManagement.ContentItems",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BranchId = c.String(maxLength: 3),
                        ImageUrl = c.String(maxLength: 1024),
                        TagLine = c.String(maxLength: 256),
                        TargetUrlText = c.String(maxLength: 256),
                        TargetUrl = c.String(maxLength: 1024),
                        CampaignId = c.String(maxLength: 1024),
                        Content = c.String(maxLength: 1024),
                        IsContentHtml = c.Boolean(nullable: false),
                        ActiveDateStart = c.DateTime(nullable: false),
                        ActiveDateEnd = c.DateTime(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("ContentManagement.ContentItems");
        }
    }
}
