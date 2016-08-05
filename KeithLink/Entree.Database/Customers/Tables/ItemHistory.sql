CREATE TABLE [Customers].[ItemHistory] (
    [Id]             BIGINT   IDENTITY (1, 1) NOT NULL,
    [BranchId]       CHAR (3) NULL,
    [CustomerNumber] CHAR (6) NULL,
    [ItemNumber]     CHAR (6) NULL,
    [CreatedUtc]     DATETIME DEFAULT (getutcdate()) NOT NULL,
    [ModifiedUtc]    DATETIME DEFAULT (getutcdate()) NOT NULL,
    [UnitOfMeasure]  CHAR (1) NULL,
    [AverageUse]     INT      DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Customers.ItemHistory] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IdxItemHistory]
    ON [Customers].[ItemHistory]([BranchId] ASC, [CustomerNumber] ASC, [ItemNumber] ASC, [UnitOfMeasure] ASC);

