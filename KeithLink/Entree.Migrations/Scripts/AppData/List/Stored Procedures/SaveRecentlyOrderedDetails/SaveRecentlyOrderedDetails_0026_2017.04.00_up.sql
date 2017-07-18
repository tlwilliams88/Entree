CREATE PROCEDURE [List].[SaveRecentlyOrderedDetails] 
    @Id                             BIGINT,
    @HeaderId  BIGINT,
    @ItemNumber                     VARCHAR(6),
    @Each                           BIT,
    @CatalogId                      VARCHAR(10),
    @LineNumber                     INT,
    @ReturnValue                    BIGINT OUTPUT
AS

	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
IF @Id > 0
    BEGIN
        UPDATE [List].[RecentlyOrderedDetails]
        SET
            [HeaderId] = @HeaderId,
            [ItemNumber] = @ItemNumber,
            [Each] = @Each,
            [CatalogId] = @CatalogId,
            [LineNumber] = @LineNumber,
            [ModifiedUtc] = GETUTCDATE()
        WHERE
            [Id] = @Id
    END
ELSE
    BEGIN
        INSERT INTO [List].[RecentlyOrderedDetails]
        (
            [HeaderId],
            [ItemNumber],
            [Each],
            [CatalogId],
            [LineNumber],
            [CreatedUtc],
            [ModifiedUtc]
        )
        VALUES
        (
            @HeaderId,
            @ItemNumber,
            @Each,
            @CatalogId,
            @LineNumber,
            GETUTCDATE(),
            GETUTCDATE()
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()