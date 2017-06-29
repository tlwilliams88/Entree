CREATE PROCEDURE [List].[GetCustomListHeaderById]
	@Id			BIGINT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT
		[Id],
		[UserId],
		[BranchId],
		[CustomerNumber],
		[Name],
        [Active],
		[CreatedUtc],
		[ModifiedUtc]
	FROM 
        [List].[CustomListHeaders] 
	WHERE	
        [Id] = @Id