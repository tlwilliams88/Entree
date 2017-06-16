CREATE TABLE [List].[RecentlyOrderedHeaders] (
    [Id]                BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [UserId]            UNIQUEIDENTIFIER NOT NULL,
    [CustomerNumber]    CHAR(6) NOT NULL,
    [BranchId]          CHAR(3) NOT NULL,
    [CreatedUtc]        DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]       DATETIME DEFAULT (getutcdate()) NOT NULL
);
GO
