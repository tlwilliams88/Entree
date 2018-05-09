CREATE VIEW [Orders].[RecommendedItemsOrdered] 
AS    
    SELECT oa.CreatedUtc,os.OrderSource,oh.BranchId,oh.CustomerNumber,oh.InvoiceNumber,oa.ControlNumber,oa.ItemNumber,id.Name,oa.UnitOfMeasure,od.ShippedQuantity
        FROM [Orders].[RecommendedItemsOrderedAnalytics] oa
        LEFT OUTER JOIN Orders.RecommendedOrderSource os ON os.OrderSourceId=oa.OrderSourceId
        LEFT OUTER JOIN Orders.OrderHistoryHeader oh ON oh.ControlNumber=oa.ControlNumber
        LEFT OUTER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id=oh.Id and od.ItemNumber=oa.ItemNumber and od.UnitOfMeasure=oa.UnitOfMeasure
        LEFT OUTER JOIN ETL.Staging_ItemData id ON id.BranchId=oh.BranchId and id.ItemId=oa.ItemNumber;
GO