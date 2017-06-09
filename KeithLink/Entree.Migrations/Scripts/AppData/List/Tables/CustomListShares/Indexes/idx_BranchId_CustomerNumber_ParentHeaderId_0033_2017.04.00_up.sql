CREATE INDEX idx_BranchId_CustomerNumber_ParentHeaderId
    ON [List].[CustomListShares] (
        [BranchId],
        [CustomerNumber],
        [ParentCustomListHeaderId]
    )
GO