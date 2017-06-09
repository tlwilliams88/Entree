CREATE TABLE [List].[InventoryValuationListHeaders]
(
	[Id]                INT             NOT NULL    PRIMARY KEY IDENTITY(1,1),
	[BranchId]          CHAR(3)         NOT NULL,
	[CustomerNumber]    CHAR(6)         NOT NULL,
	[Name]              VARCHAR(100)    NOT NULL,
	[CreatedUtc]        DATETIME        NOT NULL    DEFAULT GETUTCDATE(),
	[ModifiedUtc]       DATETIME        NOT NULL    DEFAULT GETUTCDATE()
)

