CREATE PROC [List].[SaveInventoryValuationHeader]
    @Id                BIGINT,
	@BranchId          CHAR(3),
	@CustomerNumber    CHAR(6),
	@Name              NVARCHAR(150),
	@Active		  	   BIT,
    @ReturnValue       BIGINT              OUTPUT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF @Id > 0 
      BEGIN
		UPDATE
			[List].[InventoryValuationListHeaders]
		SET
			[BranchId] = @BranchId,
			[CustomerNumber] = @CustomerNumber,
			[Name] = @Name,
			[Active] = @Active
		WHERE
			[Id] = @Id
			
        SET @ReturnValue = @Id

      END
	else
      BEGIN
        INSERT INTO
            [List].[InventoryValuationListHeaders] (
                [BranchId],
	            [CustomerNumber],
	            [Name],
				[Active],
	            [CreatedUtc],
	            [ModifiedUtc]
            ) VALUES (
                @BranchId,
                @customerNumber,
                @Name,
				1,
                GETUTCDATE(),
                GETUTCDATE()
            )			

        SET @ReturnValue = CAST(scope_identity() AS bigint)
      END
GO