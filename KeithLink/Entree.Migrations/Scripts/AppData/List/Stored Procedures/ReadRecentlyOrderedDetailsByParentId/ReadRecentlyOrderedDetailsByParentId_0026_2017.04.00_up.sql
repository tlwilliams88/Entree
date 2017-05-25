CREATE PROCEDURE [List].[ReadRecentlyOrderedDetailsByParentId] 
	@ParentRecentlyOrderedHeaderId	bigint
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[ItemNumber],
		[Each],
		[CatalogId],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[RecentlyOrderedDetails] 
	WHERE	[ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId
