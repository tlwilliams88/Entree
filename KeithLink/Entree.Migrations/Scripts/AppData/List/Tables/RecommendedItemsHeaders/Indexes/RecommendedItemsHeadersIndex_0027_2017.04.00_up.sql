CREATE UNIQUE INDEX [IX_RecommendedItemsHeaders_CustomerNumberBranchId] ON [List].[RecommendedItemsHeader]
(
    [CustomerNumber],
    [BranchId]
)