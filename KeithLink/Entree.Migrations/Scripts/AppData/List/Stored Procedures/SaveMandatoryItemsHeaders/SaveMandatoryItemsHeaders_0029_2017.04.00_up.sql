CREATE PROCEDURE [List].[SaveMandatoryItemsHeaders]
    @Id             BIGINT,
    @CustomerNumber	CHAR (6),
    @BranchId       CHAR (3),
    @Name           VARCHAR (40),
    @ReturnValue    BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
    UPDATE [List].[MandatoryItemsHeaders]
        SET
            [CustomerNumber] = @CustomerNumber,
            [BranchId] = @BranchId,
            [Name] = @Name
        WHERE
            [CustomerNumber] = @CustomerNumber
        AND [BranchId] = @BranchId
    END
ELSE
    BEGIN
        INSERT INTO [List].[MandatoryItemsHead]
        (
            [CustomerNumber],
            [BranchId],
            [Name],
            [CreatedUtc],
            [ModifiedUtc]
        )
        VALUES
        (
            @CustomerNumber,
            @BranchId,
            @Name,
            GETUTCDATE(),
            GETUTCDATE()
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()