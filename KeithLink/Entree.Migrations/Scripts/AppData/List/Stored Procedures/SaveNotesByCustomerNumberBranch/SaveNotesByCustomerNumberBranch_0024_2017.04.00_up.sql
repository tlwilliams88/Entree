CREATE PROCEDURE [List].[SaveNotesByCustomerNumberBranch] 
    @Id                     BIGINT,
    @ParentNotesHeaderId    BIGINT,
    @ItemNumber             CHAR(6),
    @Each                   BIT,
    @CatalogId              VARCHAR(10),
    @Note                   NVARCHAR(500),
    @Active                 BIT,
    @ReturnValue            BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[NotesDetails] SET
            [ParentNotesHeaderId] = @ParentNotesHeaderId,
            [ItemNumber] = @ItemNumber,
            [Each] = @Each,
            [CatalogId] = @CatalogId,
            [Note] = @Note,
            [Active] = @Active
        WHERE
            [Id] = @Id
    END
ELSE
    BEGIN
        INSERT INTO [List].[NotesDetails]
        (
            [ParentNotesHeaderId],
            [ItemNumber],
            [Each],
            [CatalogId],
            [Note],
            [Active]
        ) VALUES (
            @ParentNotesHeaderId,
            @ItemNumber,
            @Each,
            @CatalogId,
            @Note,
            @Active
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()