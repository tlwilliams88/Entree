CREATE PROCEDURE [List].[DeleteMandatoryItemDetails] 
	@Id BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [List].[DeleteMandatoryItemDetails]
	SET [Active] = 0
	WHERE [Id] = @Id;
