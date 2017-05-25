CREATE PROCEDURE [List].[GetRecommendedItemsHeaderByCustomerNumberBranch]
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[CustomerNumber],
		[BranchId],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[RecommendedItemsHeaders] 
	WHERE	[CustomerNumber] = @CustomerNumber
			AND [BranchId] = @BranchId