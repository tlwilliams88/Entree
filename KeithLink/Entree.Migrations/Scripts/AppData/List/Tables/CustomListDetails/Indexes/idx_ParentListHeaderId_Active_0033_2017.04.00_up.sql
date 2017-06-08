CREATE INDEX idx_ParentListHeaderId_Active
	ON	[List].[CustomListDetails] (
		ParentCustomListHeaderId,
		Active
	)
GO