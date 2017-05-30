CREATE PROCEDURE [List].[GetInventoryValuationListHeaderById]
	@ListId         BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[CustomerNumber],
		[BranchId],
		[Name],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[InventoryValuationListHeaders] 
	WHERE	[Id] = @ListId
