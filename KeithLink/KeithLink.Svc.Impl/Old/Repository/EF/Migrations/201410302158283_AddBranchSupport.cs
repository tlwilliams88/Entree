namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBranchSupport : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BranchSupport.BranchSupports",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BranchName = c.String(),
                        BranchId = c.String(),
                        SupportPhoneNumber = c.String(),
                        TollFreeNumber = c.String(),
                        Email = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("BranchSupport.BranchSupports");
        }
    }
}
