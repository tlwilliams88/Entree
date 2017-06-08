CREATE PROCEDURE [List].[GetCustomListHeadersByCustomerNumberBranch]
	@BranchId		CHAR(3),
	@CustomerNumber CHAR(6)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[UserId],
		[BranchId],
		[CustomerNumber],
		[Name],
        [Active],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[CustomListHeaders] 
	WHERE	
        [BranchId] = @BranchId
    AND
        [CustomerNumber] = @CustomerNumber