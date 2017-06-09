CREATE PROCEDURE [List].[AddOrUpdateMandatoryItemByCustomerNumberBranch] 
    @Id                             BIGINT,
    @ParentMandatoryItemsHeaderId   BIGINT,
    @CustomerNumber                 CHAR (6),
    @BranchId                       CHAR (3),
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
            [CustomerNumber] = @CustomerNumber,
            [BranchId] = @BranchId,
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
            [CustomerNumber],
            [BranchId],
            [ItemNumber],
            [Each],
            [CatalogId],
            [Active]
        )
        VALUES
        (
            @ParentMandatoryItemsHeaderId,
            @CustomerNumber,
            @BranchId,
            @ItemNumber,
            @Each,
            @CatalogId,
            @Active
        )
    END