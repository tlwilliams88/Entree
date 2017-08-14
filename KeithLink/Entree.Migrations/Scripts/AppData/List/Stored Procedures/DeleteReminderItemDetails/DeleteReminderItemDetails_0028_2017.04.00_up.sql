CREATE PROCEDURE [List].[DeleteReminderItemDetails] 
	@Id BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE
        [List].[ReminderDetails]
    SET
        [Active] = 0,
        [ModifiedUtc] = GETUTCDATE()
    WHERE
        [Id] = @Id