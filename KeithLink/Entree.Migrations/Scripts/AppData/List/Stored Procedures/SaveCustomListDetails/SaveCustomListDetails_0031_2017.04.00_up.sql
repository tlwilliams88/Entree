CREATE PROCEDURE [List].[SaveCustomListDetails] 
    @Id                         INT,
	@ParentCustomListHeaderId	BIGINT,
	@ItemNumber		            CHAR(6)         = NULL,
	@Each                       BIT             = NULL,
	@Par                        DECIMAL(18, 2),
    @Label                      NVARCHAR(150)   = NULL,
	@CatalogId                  VARCHAR(10)     = NULL,
    @CustomInventoryItemId      BIGINT          = NULL,
	@Active			            BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
		
    UPDATE 
        [List].[CustomListDetails]
    SET
        [ParentCustomListHeaderId] = @ParentCustomListHeaderId,
        [ItemNumber]               = @ItemNumber,
        [Each]                     = @Each,
        [Par]                      = @Par, 
        [Label]                    = @Label,
        [CatalogId]                = @CatalogId,
        [CustomInventoryItemId]    = @CustomInventoryItemId,
        [Active]                   = @Active, 
        [ModifiedUtc]              = GETUTCDATE()
    WHERE
        Id = @Id

    IF @@ROWCOUNT = 0 
      BEGIN
        INSERT INTO
            [List].[CustomListDetails] (
                [ParentCustomListHeaderId], 
                [ItemNumber],
	            [Each],
                [Par],
	            [Label],
	            [CatalogId],
	            [CustomInventoryItemId],
	            [Active],
	            [CreatedUtc],
	            [ModifiedUtc]
        ) VALUES (
        	@ParentCustomListHeaderId,
	        @ItemNumber,
	        @Each,
	        @Par,
            @Label,
	        @CatalogId,
            @CustomInventoryItemId,
	        @Active,
            GETUTCDATE(),
            GETUTCDATE()
        )
      END		