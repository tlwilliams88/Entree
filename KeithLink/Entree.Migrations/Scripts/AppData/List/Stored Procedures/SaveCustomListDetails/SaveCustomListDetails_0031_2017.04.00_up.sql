CREATE PROCEDURE [List].[SaveCustomListDetails] 
    @Id                         INT,
    @HeaderId                   BIGINT,
    @LineNumber                 INT,
    @ItemNumber                 VARCHAR(6)         = NULL,
    @Each                       BIT             = NULL,
    @Par                        DECIMAL(18, 2),
    @Label                      NVARCHAR(150)   = NULL,
    @CatalogId                  VARCHAR(10)     = NULL,
    @CustomInventoryItemId      BIGINT          = NULL,
    @Active                     BIT,
    @ReturnValue                BIGINT OUTPUT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
    IF @Id > 0 
      BEGIN
            UPDATE 
                [List].[CustomListDetails]
            SET
                [HeaderId]                 = @HeaderId,
                [LineNumber]               = @LineNumber,
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

            SET @ReturnValue = @Id
      END

    IF @@ROWCOUNT = 0 
      BEGIN
        INSERT INTO
            [List].[CustomListDetails] (
                [HeaderId], 
                [ItemNumber],
                [LineNumber],
                [Each],
                [Par],
                [Label],
                [CatalogId],
                [CustomInventoryItemId],
                [Active],
                [CreatedUtc],
                [ModifiedUtc]
        ) VALUES (
            @HeaderId,
            @ItemNumber,
            @LineNumber,
            @Each,
            @Par,
            @Label,
            @CatalogId,
            @CustomInventoryItemId,
            @Active,
            GETUTCDATE(),
            GETUTCDATE()
        )

        SET @ReturnValue = SCOPE_IDENTITY()
      END       