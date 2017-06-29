CREATE INDEX idx_BranchId_CustomerNumber
    ON [List].[NotesHeaders] ( 
        BranchId,
        CustomerNumber
    )
GO