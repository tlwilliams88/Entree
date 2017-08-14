CREATE PROCEDURE [List].[GetRecentlyViewedHeaderByUserIdCustomerNumberBranch]
	@UserId			UNIQUEIDENTIFIER,
    @BranchId       CHAR(3),
    @CustomerNumber CHAR(6)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[UserId],
		[BranchId],
		[CustomerNumber],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[RecentlyViewedHeaders] 
	WHERE	
        [UserId] = @UserId
	AND 
        [BranchId] = @BranchId
	AND 
        [CustomerNumber] = @CustomerNumber