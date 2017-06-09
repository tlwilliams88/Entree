CREATE TABLE [List].[NotesHeaders] (
    [Id]                BIGINT  PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [CustomerNumber]    CHAR(6) NULL,
    [BranchId]          CHAR(3) NULL,
    [Name]              NVARCHAR(500) NULL,
    [CreatedUtc]        DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]       DATETIME DEFAULT (getutcdate()) NOT NULL
);
GO
