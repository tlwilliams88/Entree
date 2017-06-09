CREATE TABLE [List].[ContractHeaders]
(
	[Id]				INT			NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[ContractId]		VARCHAR(8)	NULL,
	[BranchId]			CHAR(3)		NOT NULL,
	[CustomerNumber]	CHAR(6)		NOT NULL,
	[Name]				VARCHAR(30) NULL		DEFAULT 'Contract',
	[CreatedUtc]		DATETIME	NOT NULL	DEFAULT GETUTCDATE(),
	[ModifiedUtc]		DATETIME	NOT NULL	DEFAULT GETUTCDATE()
)

