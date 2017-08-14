CREATE TABLE [List].[NotesDetails] (
    [Id]                        BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [HeaderId]                  BIGINT NOT NULL,
    [ItemNumber]                VARCHAR(6) NOT NULL,
    [Each]                      BIT NULL,
    [CatalogId]                 VARCHAR(10) NOT NULL,
    [Note]                      NVARCHAR(500) NULL,
    [LineNumber]                INT,
    [CreatedUtc]                DATETIME,
    [ModifiedUtc]               DATETIME 
);
GO
