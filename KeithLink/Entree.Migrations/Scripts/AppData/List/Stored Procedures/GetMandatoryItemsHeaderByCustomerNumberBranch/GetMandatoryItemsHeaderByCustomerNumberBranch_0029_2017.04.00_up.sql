CREATE PROCEDURE [List].[GetMandatoryItemsHeaderByCustomerNumberBranch]
    @CustomerNumber CHAR (6),
    @BranchId       CHAR (3)
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [CustomerNumber],
        [BranchId],
        [Name],
        [CreatedUtc],
        [ModifiedUtc]
    FROM [List].[MandatoryItemsHeaders] 
    WHERE
        [CustomerNumber] = @CustomerNumber
    AND [BranchId] = @BranchId