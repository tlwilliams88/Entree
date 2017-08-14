CREATE PROCEDURE [List].[SaveCustomListShare] 
	@BranchId		    CHAR (3),
	@CustomerNumber	    CHAR (6),
	@HeaderId		    BIGINT,
    @ReturnValue        BIGINT OUTPUT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE
        [List].[CustomListShares]
    SET
        @ReturnValue = [Id],
        [Active] = 1,
        [ModifiedUtc] = GETUTCDATE()
    WHERE 
        [BranchId] = @BranchId
    AND
        [CustomerNumber] = @CustomerNumber
    AND
        [HeaderId] = @HeaderId


    IF @@ROWCOUNT = 0
      BEGIN
        INSERT INTO 
            [List].[CustomListShares] (
                [BranchId],
                [CustomerNumber],
                [HeaderId],
                [Active],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @BranchId,
                @CustomerNumber,
                @HeaderId,
                1,
                GETUTCDATE(),
                GETUTCDATE()
            )

        SET @ReturnValue = SCOPE_IDENTITY()
      END
GO