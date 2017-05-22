﻿CREATE TABLE [List].[ContractItems]
(
	[Id] INT PRIMARY KEY IDENTITY(1,1),
	[ParentContractHeaderId] INT NOT NULL,
	[LineNumber] INT NOT NULL,
	[ItemNumber] CHAR(10) NOT NULL,
	[FromDate] DATETIME NOT NULL,
	[ToDate] DATETIME NOT NULL,
	[Each] BIT NULL,
	[Category] VARCHAR(40) NOT NULL,
	[CatalogId] VARCHAR(24) NOT NULL,
	[CreatedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[ModifiedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE()
);