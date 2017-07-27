DROP INDEX idx_BranchId_CustomerNumber
    ON [List].[NotesHeaders] 

GO

ALTER TABLE [List].[NotesHeaders]
ALTER COLUMN [BranchId] CHAR(3) NULL

GO

ALTER TABLE [List].[NotesHeaders]
ALTER COLUMN [CustomerNumber] CHAR(6) NULL

GO

CREATE INDEX idx_BranchId_CustomerNumber
    ON [List].[NotesHeaders] ( 
        BranchId,
        CustomerNumber
    )
GO