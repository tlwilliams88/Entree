namespace KeithLink.Svc.Impl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderHistoryIndexOnOrderSystem : DbMigration
    {
        private const string NAME_INDEX = "ix_OrderHistoryheader_OrderSystem_includes";

        public override void Up()
        {
            Sql(string.Format(@"CREATE INDEX [{0}] ON [BEK_Commerce_AppData].[Orders].[OrderHistoryHeader] ([OrderSystem])  
                                INCLUDE ([Id], [BranchId], [CustomerNumber], 
                                         [InvoiceNumber], [PONumber], [ControlNumber], 
                                         [OrderStatus], [FutureItems], [ErrorStatus], 
                                         [RouteNumber], [CreatedUtc], [ModifiedUtc], 
                                         [StopNumber], [DeliveryOutOfSequence], [OriginalControlNumber], 
                                         [IsSpecialOrder], [RelatedControlNumber], [DeliveryDate], 
                                         [ScheduledDeliveryTime], [EstimatedDeliveryTime], [ActualDeliveryTime], 
                                         [OrderSubtotal]) 
                                WITH (FILLFACTOR=100, ONLINE=ON, SORT_IN_TEMPDB=ON)", NAME_INDEX));
        }
        
        public override void Down()
        {
            DropIndex("Orders.OrderHistoryHeader", NAME_INDEX);
        }
    }
}
