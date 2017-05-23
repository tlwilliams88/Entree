CREATE PROCEDURE [List].[ReadHistoryDetailsByParentId] 
	@ParentHistoryHeaderId	bigint
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[LineNumber],
		[ItemNumber],
		[Each],
		[CatalogId],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[HistoryDetails] 
	WHERE	[ParentHistoryHeaderId] = @ParentHistoryHeaderId
