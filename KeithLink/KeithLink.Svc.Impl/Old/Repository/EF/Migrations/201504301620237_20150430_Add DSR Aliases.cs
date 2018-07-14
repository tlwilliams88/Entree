namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20150430_AddDSRAliases : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BranchSupport.DsrAliases",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 200, unicode: false),
                        BranchId = c.String(nullable: false, maxLength: 3, fixedLength: true, unicode: false),
                        CustomerNumber = c.String(nullable: false, maxLength: 6, fixedLength: true, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("BranchSupport.DsrAliases");
        }
    }
}
