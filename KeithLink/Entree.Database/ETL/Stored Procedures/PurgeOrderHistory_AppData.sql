
CREATE PROCEDURE ETL.PurgeOrderHistory_AppData AS 

/*******************************************************************
* PROCEDURE: PurgeOrderHistory
* PURPOSE: Purge 6 months of order history.  Uses transactions blocks
* of 100,000 so it doesn't fill up transaction log.
* NOTES: {special set up or requirements, etc.}
* CREATED:	Jason McMillan 2015-11-25
* MODIFIED 
* DATE		AUTHOR			DESCRIPTION
*-------------------------------------------------------------------
* {date}	{developer}	{brief modification description}
*******************************************************************/

SET NOCOUNT ON;

/*****DELETE APP DATA ORDER HISTORY TRANSACTIONS*****/

--ORDER HISTORY DETAIL
DECLARE @Counter_d int;
SET @Counter_d = (
		SELECT 
			COUNT([Id])
		FROM 
			Orders.OrderHistoryDetail
		WHERE
			OrderHistoryHeader_Id IN (SELECT [Id] FROM Orders.OrderHistoryHeader WHERE CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE()))
	);

WHILE @Counter_d > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			Orders.OrderHistoryDetail
		WHERE
			OrderHistoryHeader_Id IN (SELECT [Id] FROM Orders.OrderHistoryHeader WHERE CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE()));
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_d = (
			SELECT 
				COUNT([Id])
			FROM 
				Orders.OrderHistoryDetail
			WHERE
				OrderHistoryHeader_Id IN (SELECT [Id] FROM Orders.OrderHistoryHeader WHERE CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE()))
		);
	END


--ORDER HISTORY HEADER
DECLARE @Counter_h int;
SET @Counter_h = (
		SELECT 
			COUNT([Id])
		FROM 
			Orders.OrderHistoryHeader
		WHERE
			CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE())
	);

WHILE @Counter_h > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			Orders.OrderHistoryHeader
		WHERE
			CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE())
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_h = (
			SELECT 
				COUNT([Id])
			FROM 
				Orders.OrderHistoryHeader
			WHERE
				CONVERT(date, CreatedUtc) <= DATEADD(mm, -6, GETDATE())
		);
	END