CREATE PROCEDURE [List].[ReadCustomListDetailsByParentId] 
	@HeaderId	BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [HeaderId],
	    [ItemNumber],
	    [Each],
        [Par],
	    [Label],
	    [CatalogId],
	    [CustomInventoryItemId],
        [Active],
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM 
        [List].[CustomListDetails] 
	WHERE	
        [HeaderId] = @HeaderId
	AND 
        [Active] = 1
