CREATE PROCEDURE [List].[SaveFavoriteDetails]
    @Id                         BIGINT,
    @HeaderId                   BIGINT,
    @ItemNumber                 CHAR(6),
    @LineNumber	                INT,
    @Each                       BIT             = NULL,
    @Label                      NVARCHAR(150)   = NULL,
    @CatalogId                  VARCHAR(10)     = NULL,
    @Active                     BIT,
    @ReturnValue                BIGINT          OUTPUT

AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
    IF @Id > 0 
      BEGIN
        UPDATE			
            [List].[FavoritesDetails]
        SET
            [HeaderId] = @HeaderId,
            [ItemNumber] = @ItemNumber,
		    [LineNumber]			   = @LineNumber,
            [Each] = @Each,
            [Label] = @Label,
            [CatalogId] = @CatalogId,
            [Active] = @Active,
            [ModifiedUtc] = GETUTCDATE()
        WHERE
            Id = @Id

        SET @ReturnValue = @Id
      END

    IF @@ROWCOUNT = 0
      BEGIN
        INSERT INTO
            [List].[FavoritesDetails] (
                [HeaderId],
                [ItemNumber],
                [LineNumber],
                [Each],
                [Label],
                [CatalogId],
                [Active],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @HeaderId,
                @ItemNumber,
                @LineNumber,
                @Each,
                @Label,
                @CatalogId,
                @Active,
                GETUTCDATE(),
                GETUTCDATE()
            )

        SET @ReturnValue = SCOPE_IDENTITY()
      END
