CREATE INDEX idx_HeaderId_Active
	ON	[List].[CustomListDetails] (
		ParentCustomListHeaderId,
		Active
	)
GO