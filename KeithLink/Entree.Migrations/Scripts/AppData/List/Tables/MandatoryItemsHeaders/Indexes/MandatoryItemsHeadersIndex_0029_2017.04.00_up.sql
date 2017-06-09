CREATE UNIQUE INDEX [IX_MandatoryItemsHeaders_CustomerNumberAndBranch] ON [List].[MandatoryItemsHeaders]
(
    [CustomerNumber],
    [BranchId]
)