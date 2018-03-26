CREATE NONCLUSTERED INDEX [IX_RecommendedItems_ItemNumRecom] ON [Customers].[RecommendedItems]
(
	[ItemNumber] ASC,
	[RecommendedItem] ASC
)
INCLUDE ( 	[Id],
	[Confidence],
	[ContextKey],
	[PrimaryPriceListCode],
	[SecondaryPriceListCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_RecommendedItems] ON [Customers].[RecommendedItems]
(
	[Confidence] DESC,
	[ContextKey] ASC
)
INCLUDE ( 	[Id],
	[ItemNumber],
	[RecommendedItem],
	[PrimaryPriceListCode],
	[SecondaryPriceListCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


