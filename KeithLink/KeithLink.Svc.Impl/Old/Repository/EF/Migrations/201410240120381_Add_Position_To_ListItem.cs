namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Position_To_ListItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "Position", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("List.ListItems", "Position");
        }
    }
}
