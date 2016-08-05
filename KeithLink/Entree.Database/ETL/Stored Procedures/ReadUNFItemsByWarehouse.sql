

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadUNFItemsByWarehouse]
	@warehouse nvarchar(3)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT 
		ProductNumber,
		[Description],
		Category,
		Subgroup
	FROM [ETL].[Staging_UNFIProducts]
	WHERE 
		WarehouseNumber = @warehouse
	Order by ProductNumber
END