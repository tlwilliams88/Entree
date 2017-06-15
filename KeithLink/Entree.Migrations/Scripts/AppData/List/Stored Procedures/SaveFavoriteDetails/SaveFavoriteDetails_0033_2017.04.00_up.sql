CREATE PROCEDURE [List].[SaveFavoriteDetails]
    @Id                         BIGINT,
    @HeaderId    BIGINT,
	@ItemNumber		            CHAR(6),
	@Each                       BIT,
    @Label                      NVARCHAR(150),
	@CatalogId                  VARCHAR(10),
	@Active                     BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE			
        [List].[FavoritesDetails]
    SET
        [HeaderId] = @HeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [Label] = @Label,
        [CatalogId] = @CatalogId,
        [Active] = @Active,
        [ModifiedUtc] = GETUTCDATE()
    WHERE
        Id = @Id

    IF @@ROWCOUNT = 0
      BEGIN
        INSERT INTO
            [List].[FavoritesDetails] (
                [HeaderId],
	            [ItemNumber],
	            [Each],
	            [Label],
	            [CatalogId],
	            [Active],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @HeaderId,
	            @ItemNumber,
	            @Each,
	            @Label,
	            @CatalogId,
	            @Active,
                GETUTCDATE(),
                GETUTCDATE()
            )
      END