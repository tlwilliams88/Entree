CREATE PROCEDURE [List].[ReadRecentlyViewedDetailsByHeaderId] 
    @HeaderId BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [ItemNumber],
        [Each],
        [CatalogId],
        [LineNumber],
        [CreatedUtc],
        [ModifiedUtc]
    FROM
        [List].[RecentlyViewedDetails] 
    WHERE
        [HeaderId] = @HeaderId
