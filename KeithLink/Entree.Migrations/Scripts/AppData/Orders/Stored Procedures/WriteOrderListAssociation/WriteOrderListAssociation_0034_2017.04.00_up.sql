ALTER PROCEDURE [Orders].[WriteOrderListAssociation]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@ControlNumber	NVARCHAR (40),
			@ListId			BIGINT,
			@ListType		INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

INSERT INTO [Orders].[OrderedFromList]
           ([ControlNumber]
           ,[ListId]
		   ,[ListType])
     VALUES
           (@ControlNumber
           ,@ListId
		   ,@ListType)
END