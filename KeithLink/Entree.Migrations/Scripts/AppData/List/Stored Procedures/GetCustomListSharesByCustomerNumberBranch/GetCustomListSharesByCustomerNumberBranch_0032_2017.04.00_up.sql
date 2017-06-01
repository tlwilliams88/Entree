CREATE PROCEDURE [List].[GetCustomListSharesByCustomerNumberBranch]
	@CustomerNumber	varchar(10),
	@BranchId varchar(10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
	    [Id],
	    [CustomerNumber],
	    [BranchId],
	    [ParentCustomListHeaderId],
	    [CreatedUtc],
	    [ModifiedUtc]
	FROM [List].[CustomListShares] 
	WHERE	[CustomerNumber] = @CustomerNumber
			AND [BranchId] = @BranchId
			AND [Active] = 1