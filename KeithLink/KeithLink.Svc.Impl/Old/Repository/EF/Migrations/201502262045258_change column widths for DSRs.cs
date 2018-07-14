namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changecolumnwidthsforDSRs : DbMigration
    {
        public override void Up()
        {
            AlterColumn("BranchSupport.Dsrs", "DsrNumber", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
            AlterColumn("BranchSupport.Dsrs", "EmailAddress", c => c.String(maxLength: 200, unicode: false));
            AlterColumn("BranchSupport.Dsrs", "BranchId", c => c.String(maxLength: 3, fixedLength: true, unicode: false));
            AlterColumn("BranchSupport.Dsrs", "Name", c => c.String(maxLength: 50));
            AlterColumn("BranchSupport.Dsrs", "Phone", c => c.String(maxLength: 50));
            AlterColumn("BranchSupport.Dsrs", "ImageUrl", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("BranchSupport.Dsrs", "ImageUrl", c => c.String());
            AlterColumn("BranchSupport.Dsrs", "Phone", c => c.String());
            AlterColumn("BranchSupport.Dsrs", "Name", c => c.String());
            AlterColumn("BranchSupport.Dsrs", "BranchId", c => c.String());
            AlterColumn("BranchSupport.Dsrs", "EmailAddress", c => c.String());
            AlterColumn("BranchSupport.Dsrs", "DsrNumber", c => c.String());
        }
    }
}
