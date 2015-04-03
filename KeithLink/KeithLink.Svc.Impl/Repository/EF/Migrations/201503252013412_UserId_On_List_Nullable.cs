namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserId_On_List_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("List.Lists", "UserId", c => c.Guid());
        }
        
        public override void Down()
        {
            AlterColumn("List.Lists", "UserId", c => c.Guid(nullable: false));
        }
    }
}
