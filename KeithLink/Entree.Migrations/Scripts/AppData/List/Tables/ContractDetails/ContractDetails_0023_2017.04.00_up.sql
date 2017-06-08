CREATE TABLE [List].[ContractDetails]
(
	[Id]					 INT		 NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[ParentContractHeaderId] INT		 NOT NULL,
	[LineNumber]			 INT		 NOT NULL,
	[ItemNumber]			 CHAR(6)	 NOT NULL,
	[FromDate]				 DATETIME	 NULL,
	[ToDate]				 DATETIME	 NULL,
	[Each]					 BIT		 NULL,
	[Category]				 VARCHAR(40) NOT NULL,
	[CatalogId]				 VARCHAR(10) NOT NULL,
	[CreatedUtc]			 DATETIME	 NOT NULL	DEFAULT GETUTCDATE(),
	[ModifiedUtc]			 DATETIME	 NOT NULL	DEFAULT GETUTCDATE()
)