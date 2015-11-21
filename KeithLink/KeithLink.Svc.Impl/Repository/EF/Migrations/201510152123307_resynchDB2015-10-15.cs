namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resynchDB20151015 : DbMigration
    {
        public override void Up()
        {
            DropIndex("Profile.Settings", new[] { "UserId" });
            DropColumn("Orders.OrderHistoryHeader", "OriginalControlNumber");
            DropTable("Profile.Settings");
        }
        
        public override void Down()
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
                .PrimaryKey(t => t.Id);
            
            AddColumn("Orders.OrderHistoryHeader", "OriginalControlNumber", c => c.String(maxLength: 7, fixedLength: true, unicode: false));
            CreateIndex("Profile.Settings", "UserId");
        }
    }
}
