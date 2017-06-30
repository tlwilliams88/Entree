CREATE PROCEDURE [List].[ReadNotesDetailsByParentId] 
    @HeaderId    BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [ItemNumber],
        [Each],
        [Note],
        [CatalogId],
        [LineNumber],
        [CreatedUtc],
        [ModifiedUtc]
    FROM [List].[NotesDetails] 
    WHERE   
        [HeaderId] = @HeaderId
