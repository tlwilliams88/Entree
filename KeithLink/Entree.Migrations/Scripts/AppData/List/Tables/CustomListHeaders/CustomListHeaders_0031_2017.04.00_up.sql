CREATE TABLE [List].[CustomListHeaders]
(
	[Id]				BIGINT				NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[UserId]			UNIQUEIDENTIFIER	NULL,
	[BranchId]			CHAR(3)				NOT NULL,
	[CustomerNumber]	CHAR(6)				NOT NULL,
	[Name]				NVARCHAR(100)       NOT NULL,
	[Active]			BIT					NOT NULL,
	[CreatedUtc]		DATETIME			NOT NULL,
	[ModifiedUtc]		DATETIME			NOT NULL
);

