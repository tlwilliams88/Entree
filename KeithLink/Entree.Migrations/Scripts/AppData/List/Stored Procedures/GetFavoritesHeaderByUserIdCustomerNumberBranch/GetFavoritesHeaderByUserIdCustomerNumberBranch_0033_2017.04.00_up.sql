CREATE PROCEDURE [List].[GetFavoritesHeaderByUserIdCustomerNumberBranch]
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
        [List].[FavoritesHeaders] 
	WHERE	
        [UserId] = @UserId
	AND 
        [BranchId] = @BranchId
	AND 
        [CustomerNumber] = @CustomerNumber