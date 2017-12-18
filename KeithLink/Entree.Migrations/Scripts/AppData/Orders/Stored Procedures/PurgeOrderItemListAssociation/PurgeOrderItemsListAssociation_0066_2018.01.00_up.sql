CREATE PROCEDURE [Orders].[PurgeOrderItemsListAssociation]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@PurgeDays		INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE	[Orders].OrderedItemsFromList
	WHERE
		CreatedUtc < DATEADD(day, @PurgeDays, GETDATE())
END