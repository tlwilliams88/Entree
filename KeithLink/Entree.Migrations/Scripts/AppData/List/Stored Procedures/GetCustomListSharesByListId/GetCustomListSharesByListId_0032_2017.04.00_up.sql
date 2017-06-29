CREATE PROCEDURE [List].[GetCustomListSharesByListId]
	@ListId     BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [BranchId],
	    [CustomerNumber],
	    [HeaderId] as HeaderId,
        [Active]
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM 
        [List].[CustomListShares] 
	WHERE	
        [HeaderId] = @ListId
	AND 
        [Active] = 1