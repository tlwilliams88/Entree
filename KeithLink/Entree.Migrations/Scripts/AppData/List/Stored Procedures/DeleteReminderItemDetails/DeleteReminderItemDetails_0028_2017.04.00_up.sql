CREATE PROCEDURE [List].[DeleteReminderItemDetails] 
	@CustomerNumber	NVARCHAR (10),
	@BranchId		NVARCHAR (10)
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @ParentReminderItemsHeaderId as bigint =
		(
		SELECT
			[Id]
		FROM [List].[RemindersHeaders] 
		WHERE	[CustomerNumber] = @CustomerNumber
				AND [BranchId] = @BranchId
		)

	DELETE FROM [BEK_Commerce_AppData].[List].[RemindersDetails] WHERE  [ParentRemindersHeaderId] = @ParentReminderItemsHeaderId
