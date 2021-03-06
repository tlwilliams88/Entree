ALTER PROCEDURE [List].[ReadInventoryValuationListDetailsByParentId] 
    @HeaderId   BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [HeaderId],
        [CustomInventoryItemId],
        [ItemNumber],
        [LineNumber],
        [Each],
        [Quantity],
        [CatalogId],
        [Active],
        [Label],
        [CreatedUtc],
        [ModifiedUtc]
    FROM 
        [List].[InventoryValuationListDetails] 
    WHERE   
        [HeaderId] = @HeaderId
    AND 
        [Active] = 1
