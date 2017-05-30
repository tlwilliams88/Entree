CREATE PROCEDURE [List].[ReadInventoryValuationListDetailsByParentId] 
	@ParentInventoryValuationListHeaderId	bigint
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[ItemNumber],
		[Each],
		[Quantity],
		[CatalogId],
		[Active],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[InventoryValuationListDetails] 
	WHERE	[ParentInventoryValuationListHeaderId] = @ParentInventoryValuationListHeaderId
