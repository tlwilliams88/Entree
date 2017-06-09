CREATE PROCEDURE [List].[GetRemindersHeaderByCustomerNumberBranch]
	@BranchId		CHAR(3),
	@CustomerNumber	CHAR(6)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[BranchId],
		[CustomerNumber],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[RemindersHeaders] 
	WHERE	
        [BranchId] = @BranchId
	AND 
        [CustomerNumber] = @CustomerNumber