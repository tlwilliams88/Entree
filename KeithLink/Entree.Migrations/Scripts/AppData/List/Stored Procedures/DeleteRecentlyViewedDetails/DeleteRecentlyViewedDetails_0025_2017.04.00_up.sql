CREATE PROCEDURE [List].[DeleteRecentlyViewedDetails] 
	@UserId			[uniqueidentifier], 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @ParentRecentlyViewedHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RecentlyViewedHeaders] 
		WHERE	[UserId] = @UserId
				AND [CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)

	DELETE FROM [BEK_Commerce_AppData].[List].[RecentlyViewedDetails] WHERE  [ParentRecentlyViewedHeaderId] = @ParentRecentlyViewedHeaderId
