CREATE PROCEDURE [List].[ReadNotesDetailsByParentId] 
    @ParentNotesHeaderId    BIGINT
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
        [CreatedUtc],
        [ModifiedUtc]
    FROM [List].[NotesDetails] 
    WHERE   
        [ParentNotesHeaderId] = @ParentNotesHeaderId
