CREATE TABLE [List].[NotesDetails] (
    [Id]                        BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [ParentNotesHeaderId]       BIGINT DEFAULT 0 NOT NULL,
    [ItemNumber]                VARCHAR(6) NOT NULL,
    [Each]                      BIT NULL,
    [CatalogId]                 VARCHAR(10) NOT NULL,
    [Note]                      NVARCHAR(500) NULL,
    [CreatedUtc]                DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]               DATETIME DEFAULT (getutcdate()) NOT NULL
);
GO
