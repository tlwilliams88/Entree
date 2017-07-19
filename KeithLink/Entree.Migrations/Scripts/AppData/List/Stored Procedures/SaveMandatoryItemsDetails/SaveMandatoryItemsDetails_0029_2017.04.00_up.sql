CREATE PROCEDURE [List].[SaveMandatoryItemByCustomerNumberBranch] 
    @Id                             BIGINT,
    @HeaderId                       BIGINT,
    @ItemNumber                     CHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10),
    @Active                         BIT,
    @LineNumber                     INT,
    @Quantity                       decimal(18, 2),
    @ReturnValue                    BIGINT OUTPUT
AS

IF @LineNumber = 0
	SET @LineNumber = (SELECT Count(1) FROM [List].[MandatoryItemsDetails] WHERE [HeaderId] = @HeaderId)

IF @Id > 0
    UPDATE [List].[MandatoryItemsDetails]
    SET
        [HeaderId] = @HeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId,
        [Active] = @Active,
        [LineNumber] = @LineNumber,
		[ModifiedUtc] = GETUTCDATE()
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[MandatoryItemsDetails]
    (
        [HeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [Active],
        [LineNumber],
		[CreatedUtc],
		[ModifiedUtc]
    )
    VALUES
    (
        @HeaderId,
        @ItemNumber,
        @Each,
        @CatalogId,
        @Active,
        @LineNumber,
		GETUTCDATE(),
		GETUTCDATE()
    )

SET @ReturnValue = SCOPE_IDENTITY()