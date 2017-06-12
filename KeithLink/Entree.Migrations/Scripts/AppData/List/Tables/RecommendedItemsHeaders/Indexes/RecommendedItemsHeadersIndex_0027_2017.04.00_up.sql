CREATE UNIQUE INDEX [IX_RecommendedItemsHeaders_CustomerNumberBranchId] ON [List].[RecommendedItemsHeaders]
(
    [CustomerNumber],
    [BranchId]
)