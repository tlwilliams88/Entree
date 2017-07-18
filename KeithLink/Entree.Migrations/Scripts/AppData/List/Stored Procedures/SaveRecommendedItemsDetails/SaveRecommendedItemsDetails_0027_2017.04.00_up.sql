CREATE PROCEDURE [List].[SaveRecommendedItemsDetail] 
    @Id                             BIGINT,
    @HeaderId BIGINT,
    @ItemNumber                     VARCHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10),
    @LineNumber                     INT,
    @ReturnValue                    BIGINT OUTPUT
AS

	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
IF @Id > 0
    UPDATE [List].[RecommendedItemsDetail]
    SET
        [HeaderId] = @HeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId,
        [LineNumber] = @LineNumber,
		[ModifiedUtc] = GETUTCDATE()
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[SaveRecommendedItemsDetail]
    (
        [HeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [LineNumber],
		[CreatedUtc],
		[ModifiedUtc]
    ) VALUES (
        @HeaderId,
        @ItemNumber,
        @Each,
        @CatalogId,
        @LineNumber,
		GETUTCDATE(),
		GETUTCDATE()
    )

SET @ReturnValue = SCOPE_IDENTITY()
