CREATE INDEX idx_ParentInventoryValudationHeaderId_Active
	ON [List].[InventoryValuationListDetails] (
		[ParentInventoryValuationListHeaderId],
		[Active]
	)
GO