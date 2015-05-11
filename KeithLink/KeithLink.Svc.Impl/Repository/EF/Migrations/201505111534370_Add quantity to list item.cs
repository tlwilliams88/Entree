namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addquantitytolistitem : DbMigration
    {
        public override void Up()
        {
            AddColumn("List.ListItems", "Quantity", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("List.ListItems", "Quantity");
        }
    }
}
