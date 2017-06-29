/****** Object:  StoredProcedure [ETL].[ReadInvoices]    Script Date: 10/27/2016 1:05:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


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




GO
