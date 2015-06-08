namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Modifications_to_ItemHistory_add_unitofmeasure : DbMigration
    {
        public override void Up()
        {
            DropIndex("Customers.ItemHistory", "IdxItemHistory");
            AddColumn("Customers.ItemHistory", "UnitOfMeasure", c => c.String(maxLength: 1, fixedLength: true, unicode: false));
            CreateIndex("Customers.ItemHistory", new[] { "BranchId", "CustomerNumber", "ItemNumber", "UnitOfMeasure" }, name: "IdxItemHistory");
        }
        
        public override void Down()
        {
            DropIndex("Customers.ItemHistory", "IdxItemHistory");
            DropColumn("Customers.ItemHistory", "UnitOfMeasure");
            CreateIndex("Customers.ItemHistory", new[] { "BranchId", "CustomerNumber", "ItemNumber" }, name: "IdxItemHistory");
        }
    }
}
