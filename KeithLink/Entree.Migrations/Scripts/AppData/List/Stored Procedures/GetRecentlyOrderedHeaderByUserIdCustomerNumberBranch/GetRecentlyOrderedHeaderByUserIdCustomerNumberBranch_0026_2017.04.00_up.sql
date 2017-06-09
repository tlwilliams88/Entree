CREATE PROCEDURE [List].[GetRecentlyOrderedHeaderByUserIdCustomerNumberBranch]
    @UserId         UNIQUEIDENTIFIER,
    @CustomerNumber NVARCHAR (10),
    @BranchId       NVARCHAR (10)
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    SELECT
        [Id],
        [UserId],
        [CustomerNumber],
        [BranchId],
        [Name],
        [CreatedUtc],
        [ModifiedUtc]
    FROM [List].[RecentlyOrderedHeaders] 
    WHERE
        [UserId] = @UserId
    AND [CustomerNumber] = @CustomerNumber
    AND [BranchId] = @BranchId