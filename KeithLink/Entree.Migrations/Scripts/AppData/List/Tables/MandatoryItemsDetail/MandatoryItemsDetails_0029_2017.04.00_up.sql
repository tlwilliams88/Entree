CREATE TABLE [List].[MandatoryItemsDetails] (
    [Id]                            BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [ParentMandatoryItemsHeaderId]  BIGINT DEFAULT 0 NOT NULL,
    [ItemNumber]                    CHAR(6) NOT NULL,
    [Each]                          BIT NULL,
    [CatalogId]                     VARCHAR(10) NULL,
    [Active]                        BIT DEFAULT (1) NOT NULL,
    [CreatedUtc]                    DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]                   DATETIME DEFAULT (getutcdate()) NOT NULL
);
GO