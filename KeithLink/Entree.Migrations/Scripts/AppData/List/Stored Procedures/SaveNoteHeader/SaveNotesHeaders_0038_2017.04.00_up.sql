CREATE PROC [List].[SaveNotesHeader]
    @Id             BIGINT,
    @BranchId       CHAR(3),
    @CustomerNumber CHAR(6),
    @ReturnValue    BIGINT OUTPUT
AS
    IF @Id > 0
        BEGIN 
            UPDATE [List].[NotesHeaders]
            SET
                [BranchId] = @BranchId,
                [CustomerNumber] = @CustomerNumber,
                [ModifiedUtc] = GETUTCDATE()
            WHERE
                [Id] = @Id
        END
    ELSE
        BEGIN
            INSERT INTO [List].[NotesHeaders]
            (
                [BranchId],
                [CustomerNumber],
                [CreatedUtc],
                [ModifiedUtc]
            )
            VALUES
            (
                @BranchId,
                @CustomerNumber,
                GETUTCDATE(),
                GETUTCDATE()
            )
        END

SET @ReturnValue = SCOPE_IDENTITY()
    