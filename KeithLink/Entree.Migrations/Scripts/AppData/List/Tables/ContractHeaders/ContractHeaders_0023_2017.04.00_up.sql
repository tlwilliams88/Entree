﻿CREATE TABLE [List].[ContractHeaders]
(
	[Id] int PRIMARY KEY IDENTITY(1,1),
	[ContractId] VARCHAR(50) NOT NULL,
	[Branch] CHAR(3) NOT NULL,
	[CustomerNumber] CHAR(6) NOT NULL,
	[Name] VARCHAR(30) NOT NULL,
	[CreatedAt] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	[ModifiedUtc] DATETIME NOT NULL DEFAULT GETUTCDATE()
);

