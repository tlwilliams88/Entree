CREATE INDEX idx_HeaderId_Active
	ON [List].[InventoryValuationListDetails] (
		[HeaderId],
		[Active]
	)
GO