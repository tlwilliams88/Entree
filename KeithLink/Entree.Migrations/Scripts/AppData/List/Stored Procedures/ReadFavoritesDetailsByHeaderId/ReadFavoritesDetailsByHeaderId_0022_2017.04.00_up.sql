CREATE PROCEDURE [List].[ReadFavoritesDetailsByParentId] 
    @HeaderId   BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;


    SELECT
        [Id],
        [ItemNumber],
        [LineNumber],
        [Each],
        [Label],
        [CatalogId],
        [HeaderId],
        [Active],
        [CreatedUtc],
        [ModifiedUtc]
    FROM 
        [List].[FavoritesDetails] 
    WHERE   
        [HeaderId] = @HeaderId
    AND 
        [Active] = 1
