CREATE PROCEDURE [List].[DeleteOldRecentlyOrderedDetails] 
    @HeaderId BIGINT,
    @NumberToKeep   INT
AS

DECLARE @Count AS INT = (
    SELECT 
        COUNT([Id])
    FROM 
        [List].[RecentlyOrderedDetails] 
    WHERE
        [HeaderId] = @HeaderId
)

IF(@Count > @NumberToKeep)
    DELETE FROM 
        [List].[RecentlyOrderedDetails]
    WHERE [Id] IN
    (
        SELECT TOP (@Count - @NumberToKeep) [Id]
        FROM [List].[RecentlyOrderedDetails]
        WHERE [HeaderId] = @HeaderId
        ORDER BY [ModifiedUtc] ASC
    )