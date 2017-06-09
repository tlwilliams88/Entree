CREATE PROCEDURE [List].[ReadFavoritesDetailsByParentId] 
	@ParentFavoritesHeaderId	BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[ItemNumber],
		[Each],
		[Label],
		[CatalogId],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[FavoritesDetails] 
	WHERE	
        [ParentFavoritesHeaderId] = @ParentFavoritesHeaderId
	AND 
        [Active] = 1
