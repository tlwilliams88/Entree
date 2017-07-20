CREATE PROCEDURE [List].[SaveRemindersDetails] 
    @Id             BIGINT,
    @HeaderId       BIGINT,
    @ItemNumber     CHAR(6),
    @Each           BIT,
    @CatalogId      VARCHAR(10),
    @Active         BIT,
    @LineNumber     INT,
    @ReturnValue    BIGINT OUTPUT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

	IF @LineNumber = 0
		SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)
    
IF @Id > 0
    UPDATE
        [List].[ReminderDetails]
    SET
        HeaderId = @HeaderId,
        ItemNumber = @ItemNumber,
        Each = @Each,
        CatalogId = @CatalogId,
        Active = @Active,
        LineNumber = @LineNumber,
        ModifiedUtc = GETUTCDATE()
    WHERE
        Id = @Id

ELSE
      BEGIN
        INSERT INTO
            [List].[ReminderDetails] (
                HeaderId,
                ItemNumber,
                Each,
                CatalogId,
                Active,
                LineNumber,
                CreatedUtc,
                ModifiedUtc
            ) VALUES (
                @HeaderId,
                @ItemNumber,
                @Each,
                @CatalogId,
                @Active,
                @LineNumber,
                GETUTCDATE(),
                GETUTCDATE()
            )
      END

 SET @ReturnValue = SCOPE_IDENTITY()