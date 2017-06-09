CREATE INDEX idx_BranchId_CustomerNumber
	ON [List].[InventoryValuationListDetails](
		[BranchId],
		[CustomerNumber]
	)
GO