CREATE PROCEDURE [List].[SaveNotesByCustomerNumberBranch] 
    @Id                     BIGINT,
    @HeaderId    BIGINT,
    @ItemNumber             CHAR(6),
    @Each                   BIT,
    @CatalogId              VARCHAR(10),
    @Note                   NVARCHAR(500),
    @ReturnValue            BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[NotesDetails] SET
            [HeaderId] = @HeaderId,
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
            [HeaderId],
            [ItemNumber],
            [Each],
            [CatalogId],
            [Note]
        ) VALUES (
            @HeaderId,
            @ItemNumber,
            @Each,
            @CatalogId,
            @Note
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()