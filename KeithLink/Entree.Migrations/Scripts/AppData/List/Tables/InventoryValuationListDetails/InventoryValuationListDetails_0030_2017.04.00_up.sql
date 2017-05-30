CREATE TABLE [List].[InventoryValuationListDetails]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[ParentInventoryValuationListHeaderId] INT NOT NULL,
	[ItemNumber] VARCHAR(10) NOT NULL,
	[Each] BIT NULL,
    [Quantity] DECIMAL (18, 2) DEFAULT ((0)) NOT NULL,
	[CatalogId] VARCHAR(24) NULL,
	[Active] BIT NULL,
	[CreatedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[ModifiedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE()
);