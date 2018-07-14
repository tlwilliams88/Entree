namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_Email_Templates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Configuration.EmailTemplates",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        TemplateKey = c.String(maxLength: 50),
                        Subject = c.String(),
                        IsBodyHtml = c.Boolean(nullable: false),
                        Body = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.TemplateKey);
            
        }
        
        public override void Down()
        {
            DropIndex("Configuration.EmailTemplates", new[] { "TemplateKey" });
            DropTable("Configuration.EmailTemplates");
        }
    }
}
