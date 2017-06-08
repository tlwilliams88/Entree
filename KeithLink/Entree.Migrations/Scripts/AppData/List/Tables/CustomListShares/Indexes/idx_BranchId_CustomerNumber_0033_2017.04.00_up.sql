CREATE INDEX idx_BranchId_CustomerNumber
    ON [List].[CustomListShares] (
        [BranchId],
        [CustomerNumber]
    )
GO