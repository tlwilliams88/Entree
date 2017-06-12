CREATE PROCEDURE [List].[SaveRecommendedItemsDetail] 
    @Id                             BIGINT,
    @ParentRecommendedItemsHeaderId BIGINT,
    @ItemNumber                     VARCHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10)
AS

IF @Id > 0
    UPDATE [List].[RecommendedItemsDetail]
    SET
        [ParentRecommendedItemsHeaderId] = @ParentRecommendedItemsHeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[SaveRecommendedItemsDetail]
    (
        [ParentRecommendedItemsHeaderId],
        [ItemNumber],
        [Each],
        [CatalogId]
    ) VALUES (
        @ParentRecommendedItemsHeaderId,
        @ItemNumber,
        @Each,
        @CatalogId
    )
