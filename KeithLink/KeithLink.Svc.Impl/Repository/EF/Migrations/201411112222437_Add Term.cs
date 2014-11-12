namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTerm : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Invoice.Terms",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BranchId = c.String(maxLength: 3),
                        TermCode = c.Int(nullable: false),
                        Description = c.String(maxLength: 25),
                        Age1 = c.Int(nullable: false),
                        Age2 = c.Int(nullable: false),
                        Age3 = c.Int(nullable: false),
                        Age4 = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Invoice.Terms");
        }
    }
}
