namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Mkting_Pref_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Profile.MarketingPreferences",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Email = c.String(maxLength: 150),
                        BranchId = c.String(),
                        CurrentCustomer = c.Boolean(nullable: false),
                        LearnMore = c.Boolean(nullable: false),
                        RegisteredOn = c.DateTime(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("Profile.MarketingPreferences");
        }
    }
}
