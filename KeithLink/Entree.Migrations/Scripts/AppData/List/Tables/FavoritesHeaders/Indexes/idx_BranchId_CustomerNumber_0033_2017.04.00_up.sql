CREATE INDEX idx_BranchId_CustomerNumber
    ON [List].[FavoritesHeaders] ( 
        BranchId,
        CustomerNumber
    )
GO