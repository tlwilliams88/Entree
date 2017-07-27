-- Drop index for altering columns
DROP INDEX idx_BranchId_CustomerNumber
    ON [List].[NotesHeaders] 

GO


ALTER TABLE [List].[NotesHeaders]
ALTER COLUMN [BranchId] CHAR(3) NOT NULL

GO

ALTER TABLE [List].[NotesHeaders]
ALTER COLUMN [CustomerNumber] CHAR(6) NOT NULL

GO

-- Re-add index
CREATE INDEX idx_BranchId_CustomerNumber
    ON [List].[NotesHeaders] ( 
        BranchId,
        CustomerNumber
    )
GO