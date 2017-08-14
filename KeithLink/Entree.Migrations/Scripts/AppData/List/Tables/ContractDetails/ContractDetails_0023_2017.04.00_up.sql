CREATE TABLE [List].[ContractDetails]
(
	[Id]					 BIGINT		 NOT NULL	PRIMARY KEY IDENTITY(1,1),
	[HeaderId]               BIGINT		 NOT NULL,
	[LineNumber]			 INT		 NOT NULL,
	[ItemNumber]			 VARCHAR(6)	 NOT NULL,
	[FromDate]				 DATETIME	 NULL,
	[ToDate]				 DATETIME	 NULL,
	[Each]					 BIT		 NULL,
	[Category]				 VARCHAR(40) NOT NULL,
	[CatalogId]				 VARCHAR(10) NOT NULL,
	[CreatedUtc]			 DATETIME	 NOT NULL,
	[ModifiedUtc]			 DATETIME	 NOT NULL	
)