CREATE PROCEDURE [List].[GetCustomListSharesByListId]
	@ListId			bigint
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [CustomerNumber],
	    [BranchId],
	    [ParentCustomListHeaderId],
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM [List].[CustomListShares] 
	WHERE	[ParentCustomListHeaderId] = @ListId
			AND [Active] = 1