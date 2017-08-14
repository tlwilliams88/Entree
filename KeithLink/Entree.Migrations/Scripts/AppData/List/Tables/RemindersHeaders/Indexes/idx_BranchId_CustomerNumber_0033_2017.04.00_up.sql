CREATE INDEX idx_BranchId_CustomerNumber
    ON [List].[RemindersHeaders] ( 
        BranchId,
        CustomerNumber
    )
GO