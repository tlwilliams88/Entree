CREATE PROCEDURE [List].[DeleteOldRecentlyViewedDetails] 
    @HeaderId BIGINT,
    @NumberToKeep BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    DECLARE @Count AS INT = (
        SELECT 
            COUNT([Id])
        FROM 
            [List].[RecentlyViewedDetails]
        WHERE
            [HeaderId] = @HeaderId
    )

    if(@Count > @NumberToKeep)
        DELETE FROM [List].[RecentlyViewedDetails]
            WHERE [Id] IN
            (
                SELECT TOP (@Count - @NumberToKeep) [Id]
                FROM [List].[RecentlyViewedDetails]
                WHERE [HeaderId] = @HeaderId
                ORDER BY [ModifiedUtc] ASC
            )
