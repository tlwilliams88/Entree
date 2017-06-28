CREATE TABLE [List].[RecommendedItemsHeaders] (
    [Id]                BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
    [CustomerNumber]    CHAR(6) NULL,
    [BranchId]          CHAR(3) NULL,
    [CreatedUtc]        ,
    [ModifiedUtc]       
);
GO
