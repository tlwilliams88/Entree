namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ListItemHash : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.Lists", "ListItemHash", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("List.Lists", "ListItemHash");
        }
    }
}
