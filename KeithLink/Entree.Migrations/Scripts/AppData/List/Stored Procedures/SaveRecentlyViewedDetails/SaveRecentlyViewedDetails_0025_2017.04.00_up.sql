CREATE PROCEDURE [List].[SaveRecentlyViewedDetails] 
    @Id                             BIGINT,
    @HeaderId                       BIGINT,
    @ItemNumber                     VARCHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10),
    @LineNumber                     INT,
    @ReturnValue                    BIGINT OUTPUT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

IF @Id > 0
    UPDATE [List].[RecentlyViewedDetails]
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
    INSERT INTO [List].[RecentlyViewedDetails]
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

SET @ReturnValue = SCOPE_IDENTITY();