CREATE PROCEDURE [List].[SaveRecommendedItemsDetail] 
    @Id                             BIGINT,
    @HeaderId BIGINT,
    @ItemNumber                     VARCHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10)
AS

IF @Id > 0
    UPDATE [List].[RecommendedItemsDetail]
    SET
        [HeaderId] = @HeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[SaveRecommendedItemsDetail]
    (
        [HeaderId],
        [ItemNumber],
        [Each],
        [CatalogId]
    ) VALUES (
        @HeaderId,
        @ItemNumber,
        @Each,
        @CatalogId
    )
