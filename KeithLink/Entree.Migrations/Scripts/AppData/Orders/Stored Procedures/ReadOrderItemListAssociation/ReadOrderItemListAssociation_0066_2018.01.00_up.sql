CREATE PROCEDURE [Orders].[ReadOrderItemListAssociation]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ControlNumber		NVARCHAR (40)
            ,@ItemNumber        NVARCHAR (15)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT 
		SourceList
	FROM 
		[Orders].OrderedItemsFromList
	WHERE
		ControlNumber = @ControlNumber AND
        ItemNumber = @ItemNumber
END