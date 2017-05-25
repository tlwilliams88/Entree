CREATE PROCEDURE [List].[DeleteRecentlyOrderedDetails] 
	@UserId			[uniqueidentifier], 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @ParentRecentlyOrderedHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RecentlyOrderedHeaders] 
		WHERE	[UserId] = @UserId
				AND [CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)

	DELETE FROM [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails] WHERE  [ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId
