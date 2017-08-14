CREATE TABLE [List].[InventoryValuationListHeaders]
(
	[Id]                BIGINT          NOT NULL    PRIMARY KEY IDENTITY(1,1),
	[BranchId]          CHAR(3)         NOT NULL,
	[CustomerNumber]    CHAR(6)         NOT NULL,
	[Name]              NVARCHAR(150)   NOT NULL,
	[Active]			BIT				NOT NULL,
	[CreatedUtc]        DATETIME        NOT NULL    ,
	[ModifiedUtc]       DATETIME        NOT NULL    
)

