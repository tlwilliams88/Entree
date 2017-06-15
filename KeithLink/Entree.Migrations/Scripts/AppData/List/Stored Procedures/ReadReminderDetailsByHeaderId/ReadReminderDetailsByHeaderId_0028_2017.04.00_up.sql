CREATE PROCEDURE [List].[ReadReminderDetailsByParentId] 
	@HeaderId	BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[ItemNumber],
		[Each],
		[CatalogId],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[ReminderDetails] 
	WHERE
        [HeaderId] = @HeaderId
	AND 
        [Active] = 1
