CREATE PROCEDURE [List].[GetRecentlyViewedHeaderByUserIdCustomerNumberBranch]
    @UserId         UNIQUEIDENTIFIER, 
    @CustomerNumber CHAR (6),
    @BranchId       CHAR (3)
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [UserId],
        [CustomerNumber],
        [BranchId],
        [CreatedUtc],
        [ModifiedUtc]
    FROM 
        [List].[RecentlyViewedHeaders] 
    WHERE
        [UserId] = @UserId
    AND [CustomerNumber] = @CustomerNumber
    AND [BranchId] = @BranchId
