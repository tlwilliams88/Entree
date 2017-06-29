CREATE TABLE [List].[ContractHeaders]
(
	[Id]				BIGINT		NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[ContractId]		VARCHAR(8)	NULL,
	[BranchId]			CHAR(3)		NOT NULL,
	[CustomerNumber]	CHAR(6)		NOT NULL,
	[CreatedUtc]		DATETIME	NOT NULL,
	[ModifiedUtc]		DATETIME	NOT NULL
)

