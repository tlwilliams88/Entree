ALTER PROCEDURE [List].[SaveInventoryValuationItem] 
    @Id                         BIGINT,
    @HeaderId                   BIGINT,
	@ItemNumber		            CHAR(6)         = NULL,
	@LineNumber					INT,
    @CustomInventoryItemId      BIGINT          = NULL,
	@Each                       BIT,
	@Quantity                   DECIMAL(18, 2),
	@CatalogId                  VARCHAR(10),
	@Active			            BIT,
    @Label                      NVARCHAR(150)   = NULL,
    @ReturnValue                BIGINT                  OUTPUT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
    IF @Id > 0 
      BEGIN
        UPDATE
            [List].[InventoryValuationListDetails]
        SET
            [ItemNumber] = @ItemNumber,
            [CustomInventoryItemId] = @CustomInventoryItemId,
		    [LineNumber]            = @LineNumber,
            [Each]                  = @Each,
            [Quantity]              = @Quantity, 
            [CatalogId]             = @CatalogId, 
            [Active]                = @Active,
            [Label]                 = @Label,
            [ModifiedUtc]           = GETUTCDATE()
        WHERE 
            [Id] = @Id

        SET @ReturnValue = @Id
      END

    IF @@ROWCOUNT = 0 
      BEGIN
        INSERT INTO 
            [List].[InventoryValuationListDetails] (
	            [HeaderId],
	            [CustomInventoryItemId],
	            [ItemNumber],
				[LineNumber],
	            [Each],
                [Quantity],
	            [CatalogId],
	            [Active],
                [Label],
	            [CreatedUtc],
	            [ModifiedUtc]
            ) VALUES (
                @HeaderId,
	            @CustomInventoryItemId,
	            @ItemNumber,
			    @LineNumber,
	            @Each,
                @Quantity,
	            @CatalogId,
	            @Active,
                @Label,
	            GETUTCDATE(),
	            GETUTCDATE()
            )

        SET @ReturnValue = SCOPE_IDENTITY()
      END
GO