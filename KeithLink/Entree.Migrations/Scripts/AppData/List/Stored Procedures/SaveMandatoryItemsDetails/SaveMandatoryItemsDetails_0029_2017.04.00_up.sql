CREATE PROCEDURE [List].[SaveMandatoryItemByCustomerNumberBranch] 
    @Id                             BIGINT,
    @ParentMandatoryItemsHeaderId   BIGINT,
    @ItemNumber                     CHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10),
    @Active                         BIT,
    @ReturnValue                    BIGINT OUTPUT
AS

IF @Id > 0
    UPDATE [List].[MandatoryItemsDetails]
    SET
        [ParentMAndatoryItemsHeaderId] = @ParentMandatoryItemsHeaderId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId,
        [Active] = @Active
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[MandatoryItemsDetails]
    (
        [ParentMandatoryItemsHeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [Active]
    )
    VALUES
    (
        @ParentMandatoryItemsHeaderId,
        @ItemNumber,
        @Each,
        @CatalogId,
        @Active
    )

SET @ReturnValue = SCOPE_IDENTITY()