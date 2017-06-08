﻿CREATE TABLE [List].[CustomListHeaders]
(
	[Id]				INT					NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[UserId]			UNIQUEIDENTIFIER	NULL,
	[BranchId]			CHAR(3)				NOT NULL,
	[CustomerNumber]	CHAR(6)				NOT NULL,
	[Name]				NVARCHAR(100)		NULL		DEFAULT 'Custom',
	[Active]			BIT					NOT NULL	DEFAULT (1),
	[CreatedUtc]		DATETIME			NOT NULL	DEFAULT GETUTCDATE(),
	[ModifiedUtc]		DATETIME			NOT NULL	DEFAULT GETUTCDATE()
);

