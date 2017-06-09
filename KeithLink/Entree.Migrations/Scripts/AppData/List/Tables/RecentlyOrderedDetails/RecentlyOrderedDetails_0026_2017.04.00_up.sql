CREATE TABLE [List].[RecentlyOrderedDetails] (
    [Id]                                BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [ParentRecentlyOrderedHeaderId]     BIGINT DEFAULT 0 NOT NULL,
    [ItemNumber]                        VARCHAR(6) NOT NULL,
    [Each]                              BIT NULL,
    [CatalogId]                         VARCHAR(10) NULL,
    [CreatedUtc]                        DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]                       DATETIME DEFAULT (getutcdate()) NOT NULL
);
GO