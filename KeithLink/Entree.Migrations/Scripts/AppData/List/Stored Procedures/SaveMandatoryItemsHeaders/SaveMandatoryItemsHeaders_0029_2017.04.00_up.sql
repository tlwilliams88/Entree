CREATE PROCEDURE [List].[SaveMandatoryItemsHeaders]
    @Id             BIGINT,
    @CustomerNumber	CHAR (6),
    @BranchId       CHAR (3),
    @ReturnValue    BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
    UPDATE [List].[MandatoryItemsHeaders]
        SET
            [CustomerNumber] = @CustomerNumber,
            [BranchId] = @BranchId,
			[ModifiedUtc] = GETUTCDATE()
        WHERE
            [CustomerNumber] = @CustomerNumber
        AND [BranchId] = @BranchId
    END
ELSE
    BEGIN
        INSERT INTO [List].[MandatoryItemsHeaders]
        (
            [CustomerNumber],
            [BranchId],
            [CreatedUtc],
            [ModifiedUtc]
        )
        VALUES
        (
            @CustomerNumber,
            @BranchId,
            GETUTCDATE(),
            GETUTCDATE()
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()