CREATE PROCEDURE [List].[ReadRecommendedItemDetailsByHeaderId] 
    @HeaderId bigint
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [HeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [CreatedUtc],
        [ModifiedUtc]
    FROM
        [List].[RecommendedItemsDetails] 
    WHERE
        [HeaderId] = @HeaderId
