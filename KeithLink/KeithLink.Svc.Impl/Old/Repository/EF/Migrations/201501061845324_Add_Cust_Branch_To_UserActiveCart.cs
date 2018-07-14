namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Cust_Branch_To_UserActiveCart : DbMigration
    {
        public override void Up()
        {
            AddColumn("Orders.UserActiveCarts", "CustomerId", c => c.String());
            AddColumn("Orders.UserActiveCarts", "BranchId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("Orders.UserActiveCarts", "BranchId");
            DropColumn("Orders.UserActiveCarts", "CustomerId");
        }
    }
}
