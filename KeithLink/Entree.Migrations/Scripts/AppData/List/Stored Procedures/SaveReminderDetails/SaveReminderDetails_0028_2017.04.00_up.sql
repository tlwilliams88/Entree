CREATE PROCEDURE [List].[SaveRemindersDetails] 
    @Id             BIGINT,
    @HeaderId       BIGINT,
	@ItemNumber		CHAR(6),
	@Each           BIT,
	@CatalogId      VARCHAR(10),
	@Active			BIT
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF @Id > 0
    UPDATE
        [List].[RemindersDetails]
    SET
        ParentRemindersHeaderId = @HeaderId,
        ItemNumber = @ItemNumber,
        Each = @Each,
        CatalogId = @CatalogId,
        Active = @Active,
        ModifiedUtc = GETUTCDATE()
    WHERE
        Id = @Id

ELSE
      BEGIN
        INSERT INTO
            [List].[RemindersDetails] (
                ParentRemindersHeaderId,
                ItemNumber,
                Each,
                CatalogId,
                Active,
                CreatedUtc,
                ModifiedUtc
            ) VALUES (
                @HeaderId,
                @ItemNumber,
                @Each,
                @CatalogId,
                @Active,
                GETUTCDATE(),
                GETUTCDATE()
            )
      END