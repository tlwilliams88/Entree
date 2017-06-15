CREATE PROCEDURE [List].[SaveMandatoryItemByCustomerNumberBranch] 
    @Id                             BIGINT,
    @HeaderId   BIGINT,
    @ItemNumber                     CHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10),
    @Active                         BIT,
    @ReturnValue                    BIGINT OUTPUT
AS

IF @Id > 0
    UPDATE [List].[MandatoryItemsDetails]
    SET
        [HeaderId] = @HeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId,
        [Active] = @Active
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[MandatoryItemsDetails]
    (
        [HeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [Active]
    )
    VALUES
    (
        @HeaderId,
        @ItemNumber,
        @Each,
        @CatalogId,
        @Active
    )

SET @ReturnValue = SCOPE_IDENTITY()