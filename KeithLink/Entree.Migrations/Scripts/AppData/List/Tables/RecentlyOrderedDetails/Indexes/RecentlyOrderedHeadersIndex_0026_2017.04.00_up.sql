CREATE INDEX [IX_RecentlyOrderedDetails_CustomerNumberBranch] ON [List].[RecentlyOrderedDetails]
(
    [ParentRecentlyOrderedHeaderId],
    [ItemNumber]
)