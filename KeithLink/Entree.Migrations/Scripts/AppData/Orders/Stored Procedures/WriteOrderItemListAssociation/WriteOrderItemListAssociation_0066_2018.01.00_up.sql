CREATE PROCEDURE [Orders].[WriteOrderItemListAssociation]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ControlNumber	NVARCHAR (40),
			@ItemNumber 	NVARCHAR (15),
            @SourceList     NVARCHAR (80)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [Orders].[OrderedItemsFromList]
           ([ControlNumber]
           ,[ItemNumber]
           ,[SourceList])
     VALUES
           (@ControlNumber
           ,@ItemNumber
           ,@SourceList)
END