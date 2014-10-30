namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_ListItemHash : DbMigration
    {
        public override void Up()
        {
            DropColumn("List.Lists", "ListItemHash");
        }
        
        public override void Down()
        {
            AddColumn("List.Lists", "ListItemHash", c => c.String());
        }
    }
}
