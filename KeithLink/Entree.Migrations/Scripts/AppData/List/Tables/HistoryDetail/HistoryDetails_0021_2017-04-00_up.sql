CREATE TABLE [List].[HistoryDetails] (
    [Id]					BIGINT			NOT NULL    PRIMARY KEY IDENTITY(1,1),
    [HeaderId] BIGINT          NOT NULL    ,
	[LineNumber]			INT             NOT NULL    ,
	[ItemNumber]			CHAR(6)     	NOT NULL,
	[Each]					BIT 			NULL,
	[CatalogId]				VARCHAR(10) 	NULL,
    [CreatedUtc]			DATETIME        NOT NULL    ,
    [ModifiedUtc]			DATETIME        NOT NULL    
)
GO
