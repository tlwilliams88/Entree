CREATE UNIQUE INDEX [IX_RecentlyOrderedHeaders_CustomerNumberBranch] ON [List].[RecentlyOrderedHeaders]
(
    [UserId],
    [CustomerNumber],
    [BranchId]
)