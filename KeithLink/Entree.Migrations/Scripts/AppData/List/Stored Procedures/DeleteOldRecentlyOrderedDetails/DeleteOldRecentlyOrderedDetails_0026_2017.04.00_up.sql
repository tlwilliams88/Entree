CREATE PROCEDURE [List].[DeleteOldRecentlyOrderedDetails] 
    @ParentRecentlyOrderedHeaderId BIGINT,
    @NumberToKeep   INT
AS

DECLARE @Count AS INT = (
    SELECT 
        COUNT([Id])
    FROM 
        [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails] 
    WHERE
        [ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId
)

IF(@Count > @NumberToKeep)
    DELETE FROM 
        [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails]
    WHERE [Id] IN
    (
        SELECT TOP 
            (@Count - @NumberToKeep) [Id]
        FROM 
            [BEK_Commerce_AppData].[List].[RecentlyOrderedDetails]
        WHERE 
            [ParentRecentlyOrderedHeaderId] = @ParentRecentlyOrderedHeaderId
        ORDER BY 
            [ModifiedUtc] ASC
    )