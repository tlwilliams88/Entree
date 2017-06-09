CREATE PROCEDURE [List].[ReadMandatoryItemDetailsByParentId] 
    @ParentMandatoryItemsHeaderId   BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [ItemNumber],
        [Each],
        [CatalogId],
        [CreatedUtc],
        [ModifiedUtc]
    FROM [List].[MandatoryItemsDetails] 
    WHERE
        [ParentMandatoryItemsHeaderId] = @ParentMandatoryItemsHeaderId
    AND [Active] = 1
