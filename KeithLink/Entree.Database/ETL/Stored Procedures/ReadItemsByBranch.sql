
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [ETL].[ReadItemsByBranch]
	@branchId nvarchar(3)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT DISTINCT 
		i.[ItemId] 
		,ETL.initcap([Name]) as Name 
		,ETL.initcap([Description]) as Description 
		,ETL.initcap([Brand]) as Brand 
		,[Pack] 
		,[Size] 
		,[UPC] 
		,[MfrNumber] 
		,ETL.initcap([MfrName]) as MfrName 
		,i.CategoryId 
	FROM [ETL].[Staging_ItemData] i inner join 
		ETL.Staging_Category c on i.CategoryId = c.CategoryId 
	WHERE 
		i.BranchId = @branchId AND ItemId NOT LIKE '999%' AND SpecialOrderItem <>'Y'
	Order by i.[ItemId]
END