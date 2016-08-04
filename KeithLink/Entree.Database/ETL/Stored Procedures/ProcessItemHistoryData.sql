
CREATE PROCEDURE [ETL].[ProcessItemHistoryData]       
       @NumWeeks int
AS

SET NOCOUNT ON;

TRUNCATE TABLE [Customers].[ItemHistory];

BEGIN TRANSACTION

INSERT INTO
	[Customers].[ItemHistory]
	(
		BranchId
		, CustomerNumber
		, ItemNumber
		, CreatedUtc
		, ModifiedUtc
		, UnitOfMeasure
		, AverageUse
	)
SELECT
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, GETUTCDATE()
	, GETUTCDATE()
	, od.unitOfMeasure
	, AVG(od.ShippedQuantity)
FROM 
	Orders.OrderHistoryHeader oh
	INNER JOIN Orders.OrderHistoryDetail od ON od.OrderHistoryHeader_Id = oh.Id
WHERE 
	CONVERT(DATE, oh.CreatedUtc) > DATEADD(ww, (@NumWeeks * -1), CONVERT(DATE, GETDATE()))
GROUP BY 
	oh.BranchId
	, oh.CustomerNumber
	, od.ItemNumber
	, od.unitOfMeasure

IF @@ERROR = 0 
	COMMIT TRANSACTION
ELSE	
	ROLLBACK TRANSACTION