CREATE PROCEDURE [List].[ReadHistoryDetailsByParentId] 
    @HeaderId   bigint
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [HeaderId],
        [LineNumber],
        [ItemNumber],
        [Each],
        [CatalogId],
        [CreatedUtc],
        [ModifiedUtc]
    FROM 
        [List].[HistoryDetails] 
    WHERE   
        [HeaderId] = @HeaderId
