CREATE TABLE [List].[ReminderDetails] (
    [Id]                      BIGINT            NOT NULL    PRIMARY KEY IDENTITY(1,1),
    [HeaderId] BIGINT            NOT NULL    ,
    [ItemNumber]              CHAR(6)           NOT NULL,
    [Each]                    BIT               NULL,
    [CatalogId]               VARCHAR(10)       NULL,
    [Active]                  BIT               NOT NULL    ,
    [CreatedUtc]              DATETIME          NOT NULL    ,
    [ModifiedUtc]             DATETIME          NOT NULL    
)
GO