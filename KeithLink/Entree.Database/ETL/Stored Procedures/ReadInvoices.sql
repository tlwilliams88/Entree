

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadInvoices]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		SELECT
	CustomerNumber,
	InvoiceNumber,
	ShipDate, 
	OrderDate,
	ItemNumber,
	QuantityOrdered,
	QuantityShipped,
	CatchWeightCode,
	ExtCatchWeight,
	ItemPrice,
	ExtSalesNet,
	ClassCode,
	LineNumber
FROM [ETL].[Staging_KNet_Invoice]
ORDER BY InvoiceNumber
END