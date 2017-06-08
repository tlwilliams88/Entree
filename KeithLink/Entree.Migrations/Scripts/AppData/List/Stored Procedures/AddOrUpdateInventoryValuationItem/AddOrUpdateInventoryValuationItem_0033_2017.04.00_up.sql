CREATE PROCEDURE [List].[AddOrUpdateInventoryValuationItem] 
    @Id                         BIGINT,
    @HeaderId                   BIGINT,
	@ItemNumber		            CHAR(6)         = NULL,
    @CustomInventoryItemId      BIGINT          = NULL,
	@Each                       BIT,
	@Quantity                   DECIMAL(18, 2),
	@CatalogId                  VARCHAR(10),
	@Active			            BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
			
    UPDATE
        [List].[InventoryValuationListDetails]
    SET
        [ItemNumber] = @ItemNumber,
        [CustomInventoryItemId] = @CustomInventoryItemId,
        [Each] = @Each,
        [Quantity] = @Quantity, 
        [CatalogId] = @CatalogId, 
        [Active] = @Active, 
        [ModifiedUtc] = GETUTCDATE()
    WHERE 
        [Id] = @Id

    IF @@ROWCOUNT = 0 
      BEGIN
        INSERT INTO 
            [List].[InventoryValuationListDetails] (
	            [ParentInventoryValuationListHeaderId],
	            [CustomInventoryItemId],
	            [ItemNumber],
	            [Each],
                [Quantity],
	            [CatalogId],
	            [Active],
	            [CreatedUtc],
	            [ModifiedUtc]
            ) VALUES (
                @HeaderId,
	            @CustomInventoryItemId,
	            @ItemNumber,
	            @Each,
                @Quantity,
	            @CatalogId,
	            @Active,
	            GETUTCDATE(),
	            GETUTCDATE()
            )
      END
GO