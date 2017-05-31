CREATE PROCEDURE [List].[GetCustomListHeadersByCustomerNumberBranch]
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		CONVERT(nvarchar(50),[UserId]),
		[CustomerNumber],
		[BranchId],
		[Name],
		[CreatedUtc],
		[ModifiedUtc]
	FROM [List].[CustomListHeaders] 
	WHERE	[CustomerNumber] = @CustomerNumber
			AND [BranchId] = @BranchId