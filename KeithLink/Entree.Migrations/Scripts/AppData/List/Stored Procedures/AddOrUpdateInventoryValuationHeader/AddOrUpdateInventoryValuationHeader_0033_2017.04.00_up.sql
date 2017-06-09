CREATE PROC [List].[AddOrUpdateInventoryValuationHeader]
    @Id                INT,
	@BranchId          CHAR(3),
	@CustomerNumber    CHAR(6),
	@Name              VARCHAR(100)
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE
        [List].[InventoryValuationListHeaders]
    SET
        [BranchId] = @BranchId,
        [CustomerNumber] = @CustomerNumber,
        [Name] = @Name
    WHERE
        [Id] = @Id

    IF @@ROWCOUTN > 0 
      BEGIN
        INSERT INTO
            [List].[InventoryValuationListHeaders] (
                [BranchId],
	            [CustomerNumber],
	            [Name],
	            [CreatedUtc],
	            [ModifiedUtc]
            ) VALUES (
                @BranchId,
                @customerNumber,
                @Name,
                GETUTCDATE(),
                GETUTCDATE()
            )
      END
GO