CREATE TABLE [Customers].[RecommendedItems](
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [ItemNumber] VARCHAR(6) NOT NULL,
    [RecommendedItem] VARCHAR(6) NOT NULL,
    [Confidence] DECIMAL(17, 16) NOT NULL,
    [ContextKey] INT NOT NULL,
    [PrimaryPriceListCode] VARCHAR(5) NOT NULL,
    [SecondaryPriceListCode] VARCHAR(5) NOT NULL
)
GO