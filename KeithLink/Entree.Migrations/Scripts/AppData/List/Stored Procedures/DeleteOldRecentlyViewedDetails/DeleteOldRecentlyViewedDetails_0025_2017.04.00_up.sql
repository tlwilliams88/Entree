CREATE PROCEDURE [List].[DeleteOldRecentlyViewedDetails] 
    @ParentRecentlyViewedHeaderId,
    @NumberToKeep BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    DECLARE @Count AS INT = (
        SELECT 
            count([Id])
        FROM 
            [BEK_Commerce_AppData].[List].[RecentlyViewedDetails]
        WHERE
            [ParentRecentlyViewedHeaderId] = @ParentRecentlyViewedHeaderId
    )

    if(@Count > @NumberToKeep)
        DELETE FROM [BEK_Commerce_AppData].[List].[RecentlyViewedDetails]
            WHERE [Id] IN
            (
                SELECT TOP (@Count - @NumberToKeep) [Id]
                FROM [BEK_Commerce_AppData].[List].[RecentlyViewedDetails]
                WHERE [ParentRecentlyViewedHeaderId] = @ParentRecentlyViewedHeaderId
                ORDER BY [ModifiedUtc] ASC
            )
