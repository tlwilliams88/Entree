CREATE PROCEDURE [List].[SaveCustomListHeader] 
	@Id				BIGINT,
	@UserId			UNIQUEIDENTIFIER,
	@BranchId		CHAR(3),
	@CustomerNumber CHAR(6),
	@Name			NVARCHAR(100)       = 'Custom',
	@Active         BIT, 
    @ReturnValue    BIGINT              OUTPUT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
    IF @Id > 0 
      BEGIN
	    UPDATE
		    [List].[CustomListHeaders]
	    SET
		    [Name] = @Name, 
		    [Active] = @Active, 
		    [ModifiedUtc] = GETUTCDATE()
	    WHERE
		    Id = @Id

        SET @ReturnValue = @Id
      END
	
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

        SET @ReturnValue = CAST(scope_identity() AS bigint)
	  END		