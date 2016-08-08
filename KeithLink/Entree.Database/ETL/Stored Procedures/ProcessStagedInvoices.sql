CREATE PROCEDURE [ETL].[ProcessStagedInvoices]
AS
BEGIN
	
	SET NOCOUNT ON

	CREATE TABLE #NewInvoices (InvoiceNumber varchar(20))
	CREATE TABLE #OldInvoices (InvoiceNumber varchar(20))

	/*************************************************************************
	/
	/	Populate Terms table
	/
	**************************************************************************/
	--Terms is just a lookup table, for now just empty and repopulate
	DELETE [Invoice].[Terms]

	INSERT INTO [Invoice].[Terms] (BranchId, TermCode, [Description], Age1, Age2, Age3, Age4, CreatedUtc, ModifiedUtc)
		SELECT
			Company,
			CONVERT(INTEGER, Code),
			[Description],
			CONVERT(INTEGER, Age1),
			CONVERT(INTEGER, Age2),
			CONVERT(INTEGER, Age3),
			CONVERT(INTEGER, Age4),
			GETUTCDATE(),
			GETUTCDATE()
		FROM
			[ETL].[Staging_Terms]



	/*************************************************************************
	/
	/	Create new invoices
	/
	**************************************************************************/
	--Save invoice Ids that are in today's order detail file and weren't previously there
	INSERT INTO #NewInvoices (InvoiceNumber)
		SELECT 
			DISTINCT o.InvoiceNumber 
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_OpenDetailAR o on i.InvoiceNumber = o.InvoiceNumber
		WHERE
			o.InvoiceNumber NOT IN (SELECT InvoiceNumber FROM [Invoice].[Invoices])
		

	--INSERT Header entries
	INSERT INTO [Invoice].[Invoices] (InvoiceNumber, OrderDate, InvoiceDate, CustomerNumber, CreatedUtc, ModifiedUtc, BranchId, [Type], Amount, [Status], DueDate)
		SELECT
			DISTINCT
			o.InvoiceNumber,
			CONVERT(DATETIME, i.OrderDate),
			CONVERT(DATETIME, i.ShipDate),
			o.Customer,
			GETUTCDATE(),
			GETUTCDATE(),
			o.Company,
			CASE 
				WHEN o.InvoiceNumber LIKE '%-C-%' THEN 1 
				WHEN o.InvoiceNumber LIKE '%-A-%' THEN 2
				ELSE 0
			END,
			CONVERT(decimal(18,2), o.InvoiceAmount),
			0, --Open
			CONVERT(DATETIME, o.InvoiceDue)
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_OpenDetailAR o on i.InvoiceNumber = o.InvoiceNumber inner join
			#NewInvoices n on i.InvoiceNumber = n.InvoiceNumber  

	--Create Detail Records
	INSERT INTO [Invoice].[InvoiceItems] (InvoiceId, ItemNumber, ItemPrice, LineNumber, CatchWeightCode, ClassCode, ExtCatchWeight, ExtSalesNet,QuantityOrdered, QuantityShipped, CreatedUtc, ModifiedUtc)
		SELECT
			inv.Id,
			LTRIM(RTRIM(i.ItemNumber)),
			CONVERT(DECIMAL, i.ItemPrice),
			LTRIM(RTRIM(i.LineNumber)),
			CASE WHEN i.CatchWeightCode = 'Y' THEN 1 ELSE 0 END,
			SUBSTRING(LTRIM(RTRIM(i.ClassCode)), 1,2),
			CONVERT(DECIMAL, i.ExtCatchWeight),
			CONVERT(DECIMAL, i.ExtSalesNet),
			CONVERT(INTEGER, i.QuantityOrdered),
			CONVERT(INTEGER, i.QuantityShipped),
			GETUTCDATE(),
			GETUTCDATE()
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[Invoice].Invoices inv on i.InvoiceNumber = inv.InvoiceNumber inner join
			#NewInvoices n on i.InvoiceNumber = n.InvoiceNumber 

	/*************************************************************************
	/
	/	Mark paid invoices
	/
	**************************************************************************/
	UPDATE [Invoice].[Invoices] SET [Status] = 1 WHERE InvoiceNumber in (SELECT InvoiceNumber FROM [ETL].[Staging_PaidDetail])

	/*************************************************************************
	/
	/	Create new invoice records from the paid AR file. The only time this should 
	/	have anything is on the initial load, when the detail file is generated for 
	/	past invoices
	/
	**************************************************************************/
	INSERT INTO #OldInvoices (InvoiceNumber)
		SELECT 
			DISTINCT o.InvoiceNumber 
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_PaidDetail o on i.InvoiceNumber = o.InvoiceNumber
		WHERE
			o.InvoiceNumber NOT IN (SELECT InvoiceNumber FROM [Invoice].[Invoices])

	INSERT INTO [Invoice].[Invoices] (InvoiceNumber, OrderDate, InvoiceDate, CustomerNumber, CreatedUtc, ModifiedUtc, BranchId, [Type], Amount, [Status], DueDate)
		SELECT
			DISTINCT
			o.InvoiceNumber,
			CONVERT(DATETIME, i.OrderDate),
			CONVERT(DATETIME, i.ShipDate),
			o.Customer,
			GETUTCDATE(),
			GETUTCDATE(),
			o.Company,
			CASE 
				WHEN o.InvoiceNumber LIKE '%-C-%' THEN 1 
				WHEN o.InvoiceNumber LIKE '%-A-%' THEN 2
				ELSE 0
			END,
			CONVERT(decimal(18,2), o.InvoiceAmount),
			1, --Paid
			CONVERT(DATETIME, o.InvoiceDue)
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[ETL].Staging_PaidDetail o on i.InvoiceNumber = o.InvoiceNumber inner join
			#OldInvoices n on i.InvoiceNumber = n.InvoiceNumber  

	--Create Detail Records
	INSERT INTO [Invoice].[InvoiceItems] (InvoiceId, ItemNumber, ItemPrice, LineNumber, CatchWeightCode, ClassCode, ExtCatchWeight, ExtSalesNet,QuantityOrdered, QuantityShipped, CreatedUtc, ModifiedUtc)
		SELECT
			inv.Id,
			LTRIM(RTRIM(i.ItemNumber)),
			CONVERT(DECIMAL, i.ItemPrice),
			LTRIM(RTRIM(i.LineNumber)),
			CASE WHEN i.CatchWeightCode = 'Y' THEN 1 ELSE 0 END,
			SUBSTRING(LTRIM(RTRIM(i.ClassCode)), 1,2),
			CONVERT(DECIMAL, i.ExtCatchWeight),
			CONVERT(DECIMAL, i.ExtSalesNet),
			CONVERT(INTEGER, i.QuantityOrdered),
			CONVERT(INTEGER, i.QuantityShipped),
			GETUTCDATE(),
			GETUTCDATE()
		FROM
			[ETL].Staging_KNet_Invoice i inner join
			[Invoice].Invoices inv on i.InvoiceNumber = inv.InvoiceNumber inner join
			#OldInvoices n on i.InvoiceNumber = n.InvoiceNumber 

	--Cleanup
	DROP TABLE #NewInvoices
	DROP TABLE #OldInvoices
END