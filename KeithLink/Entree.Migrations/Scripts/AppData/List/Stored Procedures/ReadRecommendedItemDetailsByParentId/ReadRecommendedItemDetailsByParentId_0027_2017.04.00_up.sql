CREATE PROCEDURE [List].[ReadRecommendedItemDetailsByParentId] 
    @ParentRecommendedItemsHeaderId bigint
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [ParentRecommendedItemsHeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [CreatedUtc],
        [ModifiedUtc]
    FROM
        [List].[RecommendedItemsDetails] 
    WHERE
        [ParentRecommendedItemsHeaderId] = @ParentRecommendedItemsHeaderId