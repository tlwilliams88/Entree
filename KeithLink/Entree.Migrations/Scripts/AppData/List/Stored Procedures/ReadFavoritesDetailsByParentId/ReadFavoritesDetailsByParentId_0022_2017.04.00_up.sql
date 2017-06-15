CREATE PROCEDURE [List].[ReadFavoritesDetailsByParentId] 
	@HeaderId	BIGINT
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
        [HeaderId] = @HeaderId
	AND 
        [Active] = 1
