namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Profile_Settings_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Profile.Settings",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        Key = c.String(nullable: false, maxLength: 100, unicode: false),
                        Value = c.String(nullable: false, maxLength: 250, unicode: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropIndex("Profile.Settings", new[] { "UserId" });
            DropTable("Profile.Settings");
        }
    }
}
