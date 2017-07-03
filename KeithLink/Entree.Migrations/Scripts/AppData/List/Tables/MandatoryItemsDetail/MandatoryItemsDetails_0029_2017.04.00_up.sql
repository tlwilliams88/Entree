CREATE TABLE [List].[MandatoryItemsDetails] (
    [Id]                            BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [HeaderId]                      BIGINT NOT NULL,
    [ItemNumber]                    CHAR(6) NOT NULL,
    [Each]                          BIT NULL,
    [CatalogId]                     VARCHAR(10) NULL,
    [LineNumber]                    INT,
    [Active]                        BIT NOT NULL,
    [CreatedUtc]                    DATETIME,
    [ModifiedUtc]                   DATETIME 
);
GO