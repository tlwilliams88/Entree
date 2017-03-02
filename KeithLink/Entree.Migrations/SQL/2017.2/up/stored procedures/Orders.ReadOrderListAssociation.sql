CREATE PROCEDURE [Orders].[ReadOrderListAssociation]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ControlNumber		NVARCHAR (40)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		ListId
	FROM 
		[Orders].Order2List
	WHERE
		ControlNumber = @ControlNumber
END