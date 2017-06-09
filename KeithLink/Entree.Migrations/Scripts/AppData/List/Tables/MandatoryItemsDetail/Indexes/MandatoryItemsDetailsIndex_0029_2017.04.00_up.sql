CREATE UNIQUE INDEX [IX_MandatoryItemsDetails_CustomerNumberAndBranch] ON [List].[MandatoryItemsDetails]
(
    [ItemNumber],
    [ParentMandatoryItemsHeaderId],
    [Active]
)
