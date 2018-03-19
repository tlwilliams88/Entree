CREATE TABLE [Customers].[RecommendedItems](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ItemNumber] VARCHAR(6) NOT NULL,
    [RecommendedItem] VARCHAR(6) NOT NULL,
    [Confidence] DECIMAL(10,9) NOT NULL,
    [ContextDescription] VARCHAR(100) NOT NULL,
    [PrimaryPriceListCode] VARCHAR(4) NOT NULL,
    [SecondaryPriceListCode] VARCHAR(4) NOT NULL
)
GO