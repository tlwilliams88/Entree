CREATE PROCEDURE [List].[SaveCustomListHeader] 
	@Id				INT,
	@UserId			UNIQUEIDENTIFIER,
	@BranchId		CHAR(3),
	@CustomerNumber CHAR(6),
	@Name			NVARCHAR(100),
	@Active         BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE
		[List].[CustomListHeaders]
	SET
		[Name] = @Name, 
		[Active] = @Active, 
		[ModifiedUtc] = GETUTCDATE()
	WHERE
		Id = @Id
	
	IF @@RowCount = 0
	  BEGIN
		INSERT INTO [List].[CustomListHeaders] (
			UserId,
			BranchId,
			CustomerNumber,
			Name,
			Active,
			CreatedUtc,
			ModifiedUtc
		) VALUES (
			@UserId,
			@BranchId,
			@CustomerNumber,
			@Name,
			@Active,
			GETUTCDATE(),
			GETUTCDATE()
		)
	  END		