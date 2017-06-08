ALTER PROCEDURE [List].[GetInventoryValuationListHeadersByCustomerNumberBranch]
	@BranchId		CHAR (3),
	@CustomerNumber	CHAR (6)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[CustomerNumber],
		[BranchId],
		[Name],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
		[List].[InventoryValuationListHeaders]
	WHERE	
		[BranchId] = @BranchId
	AND 
		[CustomerNumber] = @CustomerNumber