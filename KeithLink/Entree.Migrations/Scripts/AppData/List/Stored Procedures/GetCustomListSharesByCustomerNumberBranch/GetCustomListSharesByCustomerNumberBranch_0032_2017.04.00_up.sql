CREATE PROCEDURE [List].[GetCustomListSharesByCustomerNumberBranch]
	@BranchId       CHAR(3),
	@CustomerNumber	CHAR(6)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [BranchId],
	    [CustomerNumber],
	    [HeaderId],
        [Active],
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM 
        [List].[CustomListShares] 
	WHERE	
	    [BranchId] = @BranchId
    AND
        [CustomerNumber] = @CustomerNumber
    AND
		[Active] = 1