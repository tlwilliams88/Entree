CREATE PROCEDURE [List].[GetInventoryValuationListHeaderById]
	@Id         BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[BranchId],
		[CustomerNumber],
		[Name],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[InventoryValuationListHeaders] 
	WHERE	
        [Id] = @Id
