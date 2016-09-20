USE Bek_Commerce_Transactions;
GO

CREATE PROCEDURE dbo.BEK_PurgeOrderHistory_CSTransactions AS 

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

/*****DELETE CS TRANSACTIONS*****/
--LINE ITEMS
DECLARE @Counter_li int;
SET @Counter_li = (
		SELECT 
			COUNT(OrderFormId)
		FROM 
			LineItems
		WHERE
			OrderFormId IN (SELECT OrderFormId FROM OrderForms WHERE CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE()))
	);

WHILE @Counter_li > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			LineItems
		WHERE
			OrderFormId IN (SELECT OrderFormId FROM OrderForms WHERE CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE()))
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_li = (
			SELECT
				COUNT(OrderFormId)
			FROM 
				LineItems
			WHERE
				OrderFormId IN (SELECT OrderFormId FROM OrderForms WHERE CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE()))
		);
	END


--ORDER FORMS
DECLARE @Counter_of int;
SET @Counter_of = (
		SELECT 
			COUNT(OrderFormId)
		FROM 
			OrderForms
		WHERE
			OrderFormId IN (SELECT OrderFormId FROM OrderForms WHERE CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE()))
	);

WHILE @Counter_of > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			OrderForms
		WHERE
			OrderFormId IN (SELECT OrderFormId FROM OrderForms WHERE CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE()))
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_of = (
			SELECT
				COUNT(OrderFormId)
			FROM 
				OrderForms
			WHERE
				OrderFormId IN (SELECT OrderFormId FROM OrderForms WHERE CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE()))
		);
	END


--PURCHASE ORDERS
DECLARE @Counter_po int;
SET @Counter_po = (
		SELECT 
			COUNT(OrderGroupId)
		FROM 
			PurchaseOrders
		WHERE
			CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE())
	);

WHILE @Counter_po > 0
	BEGIN
		BEGIN TRANSACTION
		DELETE TOP(100000) FROM 
			PurchaseOrders
		WHERE
			CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE())
		IF @@ERROR = 0
			COMMIT TRANSACTION
		ELSE
			ROLLBACK TRANSACTION
		
		SET @Counter_po = (
			SELECT 
				COUNT(OrderGroupId)
			FROM 
				PurchaseOrders
			WHERE
				CONVERT(date, Created) <= DATEADD(mm, -6, GETDATE())
		);
	END
GO

