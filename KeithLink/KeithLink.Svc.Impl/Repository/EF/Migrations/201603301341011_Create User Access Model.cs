namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateUserAccessModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Customers.InternalUserAccess",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BranchId = c.String(nullable: false, maxLength: 3, fixedLength: true, unicode: false),
                        CustomerNumber = c.String(maxLength: 6, fixedLength: true, unicode: false),
                        UserId = c.Guid(nullable: false),
                        CustomerId = c.Guid(nullable: false),
                        RoleId = c.String(nullable: false, maxLength: 70, unicode: false),
                        EmailAddress = c.String(nullable: false, maxLength: 200, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.BranchId, t.CustomerNumber }, name: "IdxInternalUser")
                .Index(t => t.UserId, name: "IdxByUserId");
            
        }
        
        public override void Down()
        {
            DropIndex("Customers.InternalUserAccess", "IdxByUserId");
            DropIndex("Customers.InternalUserAccess", "IdxInternalUser");
            DropTable("Customers.InternalUserAccess");
        }
    }
}
