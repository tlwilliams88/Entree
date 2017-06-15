CREATE TABLE [List].[ReminderDetails] (
    [Id]                      BIGINT            NOT NULL    PRIMARY KEY IDENTITY(1,1),
    [HeaderId] BIGINT            NOT NULL    DEFAULT 0,
    [ItemNumber]              CHAR(6)           NOT NULL,
    [Each]                    BIT               NULL,
    [CatalogId]               VARCHAR(10)       NULL,
    [Active]                  BIT               NOT NULL    DEFAULT 1,
    [CreatedUtc]              DATETIME          NOT NULL    DEFAULT GETUTCDATE(),
    [ModifiedUtc]             DATETIME          NOT NULL    DEFAULT GETUTCDATE()
)
GO