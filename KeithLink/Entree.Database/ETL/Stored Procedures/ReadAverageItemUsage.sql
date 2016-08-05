
CREATE PROCEDURE [ETL].[ReadAverageItemUsage]	
	@NumDays int
AS

--NEED TO ADD SUMMARY COMMENTS HERE

SET NOCOUNT ON;

SELECT
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, od.unitOfMeasure
	, AVG(od.ShippedQuantity) 'AverageUse'
FROM 
	Orders.OrderHistoryHeader oh
		INNER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id = oh.Id
WHERE 
	oh.CreatedUtc > DATEADD(DD, (@NumDays * -1), GETDATE())
GROUP BY 
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, od.unitOfMeasure