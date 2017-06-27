CREATE PROCEDURE [List].[ReadCustomListDetailsByParentId] 
	@ParentCustomListHeaderId	BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [ParentCustomListHeaderId],
	    [ItemNumber],
		[LineNumber],
	    [Each],
        [Par],
	    [Label],
	    [CatalogId],
	    [CustomInventoryItemId],
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM 
        [List].[CustomListDetails] 
	WHERE	
        [ParentCustomListHeaderId] = @ParentCustomListHeaderId
	AND 
        [Active] = 1
