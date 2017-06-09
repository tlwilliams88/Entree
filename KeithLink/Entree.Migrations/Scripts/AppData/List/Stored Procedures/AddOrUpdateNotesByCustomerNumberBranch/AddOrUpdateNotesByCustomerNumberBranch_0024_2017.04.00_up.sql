CREATE PROCEDURE [List].[AddOrUpdateNotesByCustomerNumberBranch] 
    @Id                     BIGINT,
    @ParentNotesHeaderId    BIGINT,
    @CustomerNumber         CHAR(6),
    @BranchId               CHAR(3),
    @ItemNumber             CHAR(6),
    @Each                   BIT,
    @CatalogId              VARCHAR(10),
    @Note                   NVARCHAR(500),
    @Active                 BIT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[NotesDetails] SET
            [ParentNotesHeaderId] = @ParentNotesHeaderId,
            [CustomerNumber] = @CustomerNumber,
            [BranchId] = @BranchId,
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
            [CustomerNumber],
            [BranchId],
            [ItemNumber],
            [Each],
            [CatalogId],
            [Note],
            [Active]
        ) VALUES (
            @ParentNotesHeaderId,
            @CustomerNumber,
            @BranchId,
            @ItemNumber,
            @Each,
            @CatalogId,
            @note,
            @Active
        )
    END

