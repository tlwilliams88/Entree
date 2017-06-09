CREATE PROCEDURE [List].[AddOrUpdateRecentlyOrderedByUserIdCustomerNumberBranch] 
    @Id                             BIGINT,
    @ParentRecentlyOrderedHeaderId  BIGINT,
    @ItemNumber                     VARCHAR(6),
    @Each                           BIT,
    @CatalogId                      VARCHAR(10),
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[RecentlyOrderedDetails]
        SET
            [ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId,
            [ItemNumber] = @ItemNumber,
            [Each] = @Each,
            [CatalogId] = @CatalogId
        WHERE
            [Id] = @Id
    END
ELSE
    BEGIN
        INSERT INTO [List].[RecentlyOrderedDetails]
        (
            [ParentRecentlyOrderedHeaderId],
            [ItemNumber],
            [Each],
            [CatalogId],
        )
        VALUES
        (
            @ParentRecentlyOrderedHeaderId,
            @ItemNumber,
            @Each,
            @CatalogId
        )
    END