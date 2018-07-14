namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Createadditionalindexonusermessage : DbMigration
    {
        public override void Up()
        {
            CreateIndex("Messaging.UserMessages", "UserId", name: "idx_UserId");
        }
        
        public override void Down()
        {
            DropIndex("Messaging.UserMessages", "idx_UserId");
        }
    }
}
