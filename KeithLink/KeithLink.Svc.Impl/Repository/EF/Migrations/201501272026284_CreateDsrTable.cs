namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDsrTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "BranchSupport.Dsrs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DsrNumber = c.String(),
                        EmailAddress = c.String(),
                        BranchId = c.String(),
                        Name = c.String(),
                        Phone = c.String(),
                        ImageUrl = c.String(),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("BranchSupport.Dsrs");
        }
    }
}
