CREATE TABLE [List].[HistoryDetails] (
    [Id]					BIGINT			NOT NULL    PRIMARY KEY IDENTITY(1,1),
    [ParentHistoryHeaderId] BIGINT          NOT NULL    DEFAULT 0,
	[LineNumber]			INT             NOT NULL    DEFAULT 0,
	[ItemNumber]			CHAR(6)     	NOT NULL,
	[Each]					BIT 			NULL,
	[CatalogId]				VARCHAR(10) 	NULL,
    [CreatedUtc]			DATETIME        NOT NULL    DEFAULT (GETUTCDATE()),
    [ModifiedUtc]			DATETIME        NOT NULL    DEFAULT (GETUTCDATE()) 
)
GO
