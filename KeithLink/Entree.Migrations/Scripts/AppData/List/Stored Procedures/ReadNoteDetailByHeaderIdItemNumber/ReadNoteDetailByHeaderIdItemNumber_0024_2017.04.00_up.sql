CREATE PROCEDURE [List].[ReadNoteDetailByParentIdAndItemNumber] 
    @HeaderId           BIGINT,
    @ItemNumber         VARCHAR(6)
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
        [HeaderId] = @HeaderId
    AND [ItemNumber] = @ItemNumber
