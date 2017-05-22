CREATE TABLE [List].[NotesDetail] (
    [Id]					  BIGINT			PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [ParentNotesHeaderId]     BIGINT            DEFAULT 0 NOT NULL,
	[ItemNumber]			  [nvarchar](15)	NOT NULL,
	[Each]					  [bit]	    		NULL,
	[CatalogId]				  [nvarchar](24)	NULL,
	[Note]				      [nvarchar](max)	NULL,
		[Active]                BIT            NULL,
    [CreatedUtc]			  DATETIME			DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]			  DATETIME          DEFAULT (getutcdate()) NOT NULL
);
GO
