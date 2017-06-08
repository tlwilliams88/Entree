CREATE TABLE [List].[CustomListDetails]
(
	[Id]						INT				NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[ParentCustomListHeaderId]	INT				NOT NULL,
	[ItemNumber]				CHAR(6)		    NULL,
	[Each]						BIT				NULL,
    [Par]						DECIMAL (18, 2) NOT NULL	DEFAULT (0),
	[Label]						NVARCHAR(150)	NULL,
	[CatalogId]					VARCHAR(10)		NULL,
	[CustomInventoryItemId]		BIGINT			NULL,   
	[Active]					BIT				NOT NULL	DEFAULT (1),
	[CreatedUtc]				DATETIME		NOT NULL	DEFAULT GETUTCDATE(),
	[ModifiedUtc]				DATETIME		NOT NULL	DEFAULT GETUTCDATE()
);