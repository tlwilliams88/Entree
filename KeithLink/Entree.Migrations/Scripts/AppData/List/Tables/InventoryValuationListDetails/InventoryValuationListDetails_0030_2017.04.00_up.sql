﻿CREATE TABLE [List].[InventoryValuationListDetails]
(
	[Id]                                    BIGINT          NOT NULL    PRIMARY KEY IDENTITY(1,1),
	[HeaderId]  BIGINT          NOT NULL,
	[CustomInventoryItemId]		            BIGINT			NULL,   
	[ItemNumber]                            CHAR(6)         NOT NULL,
	[Each]                                  BIT             NULL,
    [Quantity]                              DECIMAL (18, 2) NOT NULL    DEFAULT 0,
	[CatalogId]                             VARCHAR(10)     NULL,
	[Active]                                BIT DEFAULT (1) NOT NULL,
	[CreatedUtc]                            DATETIME        NOT NULL    DEFAULT GETUTCDATE(),
	[ModifiedUtc]                           DATETIME        NOT NULL    DEFAULT GETUTCDATE()
)