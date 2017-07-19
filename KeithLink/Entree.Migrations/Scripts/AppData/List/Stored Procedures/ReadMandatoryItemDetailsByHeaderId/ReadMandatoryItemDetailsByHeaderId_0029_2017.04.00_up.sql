CREATE PROCEDURE [List].[ReadMandatoryItemDetailsByParentId] 
    @HeaderId   BIGINT
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [HeaderId],
        [ItemNumber],
        [Each],
        [CatalogId],
        [LineNumber],
        [Active],
        [Quantity],
        [CreatedUtc],
        [ModifiedUtc]
    FROM [List].[MandatoryItemsDetails] 
    WHERE
        [HeaderId] = @HeaderId
    AND [Active] = 1
