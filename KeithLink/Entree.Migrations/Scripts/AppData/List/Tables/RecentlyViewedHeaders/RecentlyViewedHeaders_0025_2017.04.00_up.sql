CREATE TABLE [List].[RecentlyViewedHeaders] (
    [Id]                BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [UserId]            UNIQUEIDENTIFIER NULL,
    [CustomerNumber]    CHAR(6) NULL,
    [BranchId]          CHAR(3) NULL,
    [CreatedUtc]        DATETIME ,
    [ModifiedUtc]       DATETIME 
);
GO
