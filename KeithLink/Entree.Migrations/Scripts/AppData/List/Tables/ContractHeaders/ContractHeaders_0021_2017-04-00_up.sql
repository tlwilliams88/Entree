﻿CREATE TABLE [List].[ContractHeaders]
(
	[Id] int PRIMARY KEY,
	[ContractId] VARCHAR(50) NOT NULL,
	[Branch] CHAR(3) NOT NULL,
	[CustomerNumber] CHAR(6) NOT NULL,
	[Name] VARCHAR(30) NOT NULL,
	[CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
	[ModifiedOn] DATETIME NOT NULL DEFAULT GETDATE()
);

