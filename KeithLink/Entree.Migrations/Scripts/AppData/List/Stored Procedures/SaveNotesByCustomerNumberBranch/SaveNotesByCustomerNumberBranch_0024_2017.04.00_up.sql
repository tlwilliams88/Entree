CREATE PROCEDURE [List].[SaveNotesByCustomerNumberBranch] 
    @Id                     BIGINT,
    @HeaderId               BIGINT,
    @ItemNumber             CHAR(6),
	@LineNumber					        INT,
    @Each                   BIT,
    @CatalogId              VARCHAR(10),
    @Note                   NVARCHAR(500),
    @ReturnValue            BIGINT OUTPUT
AS

	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
IF @Id > 0
    BEGIN
        UPDATE [List].[NotesDetails] SET
            [HeaderId] = @HeaderId,
            [ItemNumber] = @ItemNumber,
            [Each] = @Each,
            [CatalogId] = @CatalogId,
            [Note] = @Note,
			[ModifiedUtc] = GETUTCDATE()
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
            [Note],
			[CreatedUtc],
			[ModifiedUtc]
        ) VALUES (
            @HeaderId,
            @ItemNumber,
            @Each,
            @CatalogId,
            @Note,
			GETUTCDATE(),
			GETUTCDATE()
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()