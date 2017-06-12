CREATE PROCEDURE [List].[AddOrUpdateRecentlyViewedDetails] 
    @Id                             BIGINT,
    @ParentRecentlyViewedHeaderId   BIGINT,
    @ItemNumber                     VARCHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10)
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

IF @Id > 0
    UPDATE [List].[RecentlyViewedDetails]
    SET
        [ParentRecentlyViewedHeaderId] = @ParentRecentlyViewedHeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[RecentlyViewedDetails]
    (
        [ParentRecentlyViewedHeaderId],
        [ItemNumber],
        [Each],
        [CatalogId] 
    ) VALUES (
        @ParentRecentlyViewedHeaderId,
        @ItemNumber,
        @Each,
        @CatalogId 
    )