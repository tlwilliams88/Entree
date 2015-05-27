namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ItemHistory_Table : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Customers.ItemHistory",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        BranchId = c.String(maxLength: 3, fixedLength: true, unicode: false),
                        CustomerNumber = c.String(maxLength: 6, fixedLength: true, unicode: false),
                        ItemNumber = c.String(maxLength: 6, fixedLength: true, unicode: false),
                        EightWeekAverage = c.Int(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false),
                        ModifiedUtc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.BranchId, t.CustomerNumber, t.ItemNumber }, name: "IdxItemHistory");
            
        }
        
        public override void Down()
        {
            DropIndex("Customers.ItemHistory", "IdxItemHistory");
            DropTable("Customers.ItemHistory");
        }
    }
}
