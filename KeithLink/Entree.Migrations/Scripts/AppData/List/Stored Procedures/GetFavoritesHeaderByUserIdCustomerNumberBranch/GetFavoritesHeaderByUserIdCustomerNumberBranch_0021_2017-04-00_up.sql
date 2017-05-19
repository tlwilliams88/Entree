CREATE PROCEDURE [List].[GetFavoritesHeaderByUserIdCustomerNumberBranch]
	@UserId			[uniqueidentifier], 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		CONVERT(nvarchar(max),[UserId]),
		[CustomerNumber],
		[BranchId],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[FavoritesHeader] 
	WHERE	[UserId] = @UserId
	        AND [CustomerNumber] = @CustomerNumber
			AND [BranchId] = @BranchId