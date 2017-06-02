CREATE TABLE [List].[FavoritesDetails] (
    [Id]					  BIGINT			PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [ParentFavoritesHeaderId] BIGINT            DEFAULT 0 NOT NULL,
	[ItemNumber]			  [nvarchar](15)	NOT NULL,
	[Each]					  [bit]	    		NULL,
	[Label] [varchar](150) NULL,
	[CatalogId]				  [nvarchar](24)	NULL,
	[Active] BIT DEFAULT (1) NOT NULL,
    [CreatedUtc]			  DATETIME			DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]			  DATETIME          DEFAULT (getutcdate()) NOT NULL
);
GO
