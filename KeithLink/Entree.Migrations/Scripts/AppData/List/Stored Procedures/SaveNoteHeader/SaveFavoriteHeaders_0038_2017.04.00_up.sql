CREATE PROC [List].[SaveNotesHeader]
    @UserId         UNIQUEIDENTIFIER,
    @BranchId       CHAR(3),
    @CustomerNumber CHAR(6)
AS
    IF NOT EXISTS(SELECT 
                    'X'
                  FROM
                    [List].[NotesHeaders]
                  WHERE
                    [UserId] = @UserId
                  AND
                    [BranchId] = @BranchId
                  AND
                    [CustomerNumber] = @CustomerNumber)
        INSERT INTO 
            [List].[NotesHeaders] (
                [UserId],
                [BranchId],
                [CustomerNumber],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @UserId,
                @BranchId,
                @CustomerNumber,
                GETUTCDATE(),
                GETUTCDATE()
            )

        RETURN @@IDENTITY
GO
    