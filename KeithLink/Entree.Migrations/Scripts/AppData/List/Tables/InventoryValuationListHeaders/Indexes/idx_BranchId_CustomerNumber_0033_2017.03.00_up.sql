CREATE INDEX idx_BranchId_CustomerNumber
	ON [List].[InventoryValuationListHeaders](
		[BranchId],
		[CustomerNumber]
	)
GO