CREATE INDEX idx_BranchId_CustomerNumber_HeaderId
    ON [List].[CustomListShares] (
        [BranchId],
        [CustomerNumber],
        [HeaderId]
    )
GO