namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Dates_To_ListItems : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "FromDate", c => c.DateTime());
            AddColumn("List.ListItems", "ToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("List.ListItems", "ToDate");
            DropColumn("List.ListItems", "FromDate");
        }
    }
}
