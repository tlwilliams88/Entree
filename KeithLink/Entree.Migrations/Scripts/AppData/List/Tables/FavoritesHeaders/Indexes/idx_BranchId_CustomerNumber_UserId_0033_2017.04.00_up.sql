CREATE INDEX idx_BranchId_CustomerNumber_UserId
	ON [List].[FavoritesHeaders] (
		[BranchId],
		[CustomerNumber],
		[UserId]
	)
GO