namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create_PasswordResetRequest_table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Profile.PasswordResetRequests",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        Token = c.String(maxLength: 300),
                        Expiration = c.DateTime(nullable: false),
                        Processed = c.Boolean(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Profile.PasswordResetRequests");
        }
    }
}
