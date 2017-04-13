CREATE PROCEDURE [Orders].[DeleteOrderListAssociation]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ControlNumber	NVARCHAR (40)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DELETE FROM [Orders].[OrderedFromList]
    WHERE ControlNumber = @ControlNumber
END