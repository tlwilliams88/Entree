CREATE PROCEDURE [List].[DeleteMandatoryItemDetails] 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @ParentMandatoryItemsHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[MandatoryItemsHeaders] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)

	DELETE FROM [BEK_Commerce_AppData].[List].[MandatoryItemsDetails] WHERE  [ParentMandatoryItemsHeaderId] = @ParentMandatoryItemsHeaderId
