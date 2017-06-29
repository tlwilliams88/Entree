CREATE PROC [List].[DeleteFavoriteDetail]
    @Id     BIGINT
AS
    UPDATE 
        [List].[FavoritesDetails]
    SET
        [Active] = 0,
        [ModifiedUtc] = GETUTCDATE()
    WHERE
        [Id] = @Id
GO