CREATE PROCEDURE [List].[SaveRecentlyOrderedDetails] 
    @Id                             BIGINT,
    @HeaderId  BIGINT,
    @ItemNumber                     VARCHAR(6),
    @Each                           BIT,
    @CatalogId                      VARCHAR(10)
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[RecentlyOrderedDetails]
        SET
            [HeaderId] = @HeaderId,
            [ItemNumber] = @ItemNumber,
            [Each] = @Each,
            [CatalogId] = @CatalogId,
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
			[CreatedUtc],
			[ModifiedUtc]
        )
        VALUES
        (
            @HeaderId,
            @ItemNumber,
            @Each,
            @CatalogId,
			GETUTCDATE(),
			GETUTCDATE()
        )
    END