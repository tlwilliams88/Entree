CREATE PROCEDURE [List].[ReadRecentlyViewedDetailsByParentId] 
    @ParentRecentlyViewedHeaderId BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [ItemNumber],
        [Each],
        [CatalogId],
        [CreatedUtc],
        [ModifiedUtc]
    FROM
        [List].[RecentlyViewedDetails] 
    WHERE
        [ParentRecentlyViewedHeaderId] = @ParentRecentlyViewedHeaderId
