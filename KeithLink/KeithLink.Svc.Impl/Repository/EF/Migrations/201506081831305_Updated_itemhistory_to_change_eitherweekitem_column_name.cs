namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updated_itemhistory_to_change_eitherweekitem_column_name : DbMigration
    {
        public override void Up()
        {
            AddColumn("Customers.ItemHistory", "AverageUse", c => c.Int(nullable: false));
            DropColumn("Customers.ItemHistory", "EightWeekAverage");
        }
        
        public override void Down()
        {
            AddColumn("Customers.ItemHistory", "EightWeekAverage", c => c.Int(nullable: false));
            DropColumn("Customers.ItemHistory", "AverageUse");
        }
    }
}
