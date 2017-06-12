CREATE PROCEDURE [List].[AddOrUpdateMandatoryItemByCustomerNumberBranch] 
    @Id                             BIGINT,
    @ParentMandatoryItemsHeaderId   BIGINT,
    @ItemNumber                     CHAR (6),
    @Each                           BIT,
    @CatalogId                      VARCHAR (10),
    @Active                         BIT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[MandatoryItemsDetails]
        SET
            [ParentMAndatoryItemsHeaderId] = @ParentMandatoryItemsHeaderId,
            [ItemNumber] = @ItemNumber,
            [Each] = @Each,
            [CatalogId] = @CatalogId,
            [Active] = @Active
        WHERE
            [Id] = @Id
    END
ELSE
    BEGIN
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
    END