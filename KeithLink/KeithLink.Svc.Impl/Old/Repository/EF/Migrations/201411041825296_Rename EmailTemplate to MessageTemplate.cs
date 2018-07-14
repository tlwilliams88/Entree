namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameEmailTemplatetoMessageTemplate : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "Configuration.EmailTemplates", newName: "MessageTemplates");
            AddColumn("Configuration.MessageTemplates", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Configuration.MessageTemplates", "Type");
            RenameTable(name: "Configuration.MessageTemplates", newName: "EmailTemplates");
        }
    }
}
