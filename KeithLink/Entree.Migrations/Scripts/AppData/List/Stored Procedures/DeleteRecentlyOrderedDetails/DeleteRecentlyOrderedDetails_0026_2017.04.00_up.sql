CREATE PROCEDURE [List].[DeleteRecentlyOrderedDetails] 
    @Id BIGINT
AS
    DELETE FROM [List].[RecentlyOrderedDetails]
    WHERE [Id] = @Id