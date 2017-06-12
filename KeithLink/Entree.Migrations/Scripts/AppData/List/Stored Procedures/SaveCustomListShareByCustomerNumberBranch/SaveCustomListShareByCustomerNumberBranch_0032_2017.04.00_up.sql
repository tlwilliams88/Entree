CREATE PROCEDURE [List].[SaveCustomListShareByCustomerNumberBranch] 
	@BranchId		                CHAR (3),
	@CustomerNumber	                CHAR (6),
	@ParentCustomListHeaderId		BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE
        [List].[CustomListShares]
    SET
        [Active] = 1,
        [ModifiedUtc] = GETUTCDATE()
    WHERE 
        [BranchId] = @BranchId
        AND
        [CustomerNumber] = @CustomerNumber
        AND
        [ParentCustomListHeaderId] = @ParentCustomListHeaderId


    IF @@ROWCOUNT = 0
      BEGIN
        INSERT INTO 
            [List].[CustomListShares] (
                [BranchId],
                [CustomerNumber],
                [ParentCustomListHeaderId],
                [Active],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @BranchId,
                @CustomerNumber,
                @ParentCustomListHeaderId,
                1,
                GETUTCDATE(),
                GETUTCDATE()
            )
      END
GO