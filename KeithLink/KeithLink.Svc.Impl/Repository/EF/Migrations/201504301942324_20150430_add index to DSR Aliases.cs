namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20150430_addindextoDSRAliases : DbMigration
    {
        public override void Up()
        {
            AddColumn("BranchSupport.DsrAliases", "DsrNumber", c => c.String(nullable: false, maxLength: 6, fixedLength: true, unicode: false));
            CreateIndex("BranchSupport.DsrAliases", "UserId");
            DropColumn("BranchSupport.DsrAliases", "CustomerNumber");
        }
        
        public override void Down()
        {
            AddColumn("BranchSupport.DsrAliases", "CustomerNumber", c => c.String(nullable: false, maxLength: 6, fixedLength: true, unicode: false));
            DropIndex("BranchSupport.DsrAliases", new[] { "UserId" });
            DropColumn("BranchSupport.DsrAliases", "DsrNumber");
        }
    }
}
