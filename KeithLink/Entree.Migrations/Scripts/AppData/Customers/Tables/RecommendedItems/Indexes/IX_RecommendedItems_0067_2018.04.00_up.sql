CREATE NONCLUSTERED INDEX [IX_RecommendedItems]
    ON [Customers].[RecommendedItems] ([ContextKey])
    INCLUDE ([Id],[ItemNumber],[RecommendedItem],[Confidence],[PrimaryPriceListCode],[SecondaryPriceListCode])
GO