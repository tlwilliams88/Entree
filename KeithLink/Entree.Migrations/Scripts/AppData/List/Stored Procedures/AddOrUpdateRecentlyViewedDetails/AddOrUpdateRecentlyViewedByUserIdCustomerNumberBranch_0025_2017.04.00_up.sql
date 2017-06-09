CREATE PROCEDURE [List].[AddOrUpdateRecentlyViewedDetails] 
    @Id             BIGINT,
    @UserId         UNIQUEIDENTIFIER, 
    @CustomerNumber CHAR (6),
    @BranchId       CHAR (3),
    @ItemNumber     VARCHAR (6),
    @Each           BIT,
    @CatalogId      VARCHAR (10),
AS
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

IF @Id > 0
    UPDATE [List].[RecentlyViewedDetails]
    SET
        [UserId] = @UserId,
        [CustomerNumber] = @CustomerNumber,
        [BranchId] = @BranchId,
        [ItemNumber] = @ItemNumber,
        [Each] = @Each,
        [CatalogId] = @CatalogId
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[RecentlyViewedDetails]
    (
        [UserId],
        [CustomerNumber],
        [BranchId],
        [ItemNumber],
        [Each],
        [CatalogId] 
    ) VALUES (
        @UserId,
        @CustomerNumber,
        @BranchId,
        @ItemNumber,
        @Each,
        @CatalogId 
    )