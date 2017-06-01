﻿CREATE TABLE [List].[CustomListDetails]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[ParentCustomListHeaderId] INT NOT NULL,
	[ItemNumber] NVARCHAR(10) NOT NULL,
	[Each] BIT NULL,
    [Par] DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
	[CatalogId] NVARCHAR(24) NULL,
	[CustomInventoryItemId] bigint null,
	[Active] BIT DEFAULT (1) NOT NULL,
	[CreatedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[ModifiedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE()
);