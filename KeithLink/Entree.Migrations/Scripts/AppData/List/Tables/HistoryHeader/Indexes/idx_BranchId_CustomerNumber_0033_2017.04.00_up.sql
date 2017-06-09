CREATE UNIQUE INDEX idx_BranchId_CustomerNumber
	ON [List].[HistoryHeaders] (
		[BranchId],
		[CustomerNumber]
	)
GO