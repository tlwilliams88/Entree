ALTER VIEW [Orders].[RecommendedItemsOrdered] 
AS    
    SELECT 
		oa.CreatedUtc,
		os.OrderSource,
		CASE oh.BranchId
				WHEN 'FDF' THEN '01'
				WHEN 'FEL' THEN '02'
				WHEN 'FHS' THEN '03'
				WHEN 'FAM' THEN '04'
				WHEN 'FLR' THEN '05'
				WHEN 'FOK' THEN '06'
				WHEN 'FSA' THEN '07'
				WHEN 'FAQ' THEN '08'
				WHEN 'FAR' THEN '09'
        END AS BranchId,
		oh.CustomerNumber,
		oh.InvoiceNumber,
		oa.ControlNumber,
		oa.ItemNumber,
		id.Name,
		oa.UnitOfMeasure,
		od.ShippedQuantity,
		od.SellPrice,
		oa.ProductGroupingInsightKey,
		oa.CustomerInsightVersionKey
    FROM [Orders].[RecommendedItemsOrderedAnalytics] oa
        LEFT OUTER JOIN Orders.RecommendedOrderSource os ON os.OrderSourceId=oa.OrderSourceId
        LEFT OUTER JOIN Orders.OrderHistoryHeader oh ON oh.ControlNumber=oa.ControlNumber
        LEFT OUTER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id=oh.Id and od.ItemNumber=oa.ItemNumber and od.UnitOfMeasure=oa.UnitOfMeasure
        LEFT OUTER JOIN ETL.Staging_ItemData id ON id.BranchId=oh.BranchId and id.ItemId=oa.ItemNumber;