CREATE TABLE [List].[HistoryDetail] (
    [Id]					BIGINT			PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [ParentHistoryHeaderId] BIGINT          DEFAULT 0 NOT NULL,
	[LineNumber]			[int]			NOT NULL DEFAULT ((0)),
	[ItemNumber]			[nvarchar](15)	NOT NULL,
	[Each]					[bit]			NULL,
	[CatalogId]				[nvarchar](24)	NULL,
    [CreatedUtc]			DATETIME        DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]			DATETIME        DEFAULT (getutcdate()) NOT NULL
);
GO
