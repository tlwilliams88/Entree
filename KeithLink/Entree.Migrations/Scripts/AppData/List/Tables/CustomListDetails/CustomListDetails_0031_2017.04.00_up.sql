CREATE TABLE [List].[CustomListDetails]
(
	[Id]						BIGINT			NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[HeaderId]	                BIGINT			NOT NULL,
	[ItemNumber]				CHAR(6)		    NULL,
	[Each]						BIT				NULL,
    [Par]						DECIMAL (18, 2) NOT NULL,
	[Label]						NVARCHAR(150)	NULL,
	[CatalogId]					VARCHAR(10)		NULL,
	[CustomInventoryItemId]		BIGINT			NULL,   
	[Active]					BIT				NOT NULL,
	[CreatedUtc]				DATETIME		NOT NULL,
	[ModifiedUtc]				DATETIME		NOT NULL
);