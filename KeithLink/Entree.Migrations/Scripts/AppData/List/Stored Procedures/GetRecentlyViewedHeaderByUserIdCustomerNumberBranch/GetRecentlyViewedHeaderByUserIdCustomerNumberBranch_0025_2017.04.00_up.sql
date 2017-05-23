CREATE PROCEDURE [List].[GetRecentlyViewedHeaderByUserIdCustomerNumberBranch]
	@UserId			[uniqueidentifier], 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		CONVERT(nvarchar(50),[UserId]),
		[CustomerNumber],
		[BranchId],
		[Name],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[RecentlyViewedHeaders] 
	WHERE	[UserId] = @UserId
	        AND [CustomerNumber] = @CustomerNumber
			AND [BranchId] = @BranchId