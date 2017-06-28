CREATE PROCEDURE [List].[SaveRecommendedItemHeaderByCustomerNumberBranch] 
    @Id             BIGINT,
    @CustomerNumber CHAR (6),
    @BranchId       CHAR (3),
    @ReturnValue                    BIGINT OUTPUT
AS

IF @Id > 0
    UPDATE [List].[RecommendedItemsHeaders]
    SET
        [CustomerNumber] = @CustomerNumber,
        [BranchId] = @BranchId,
		[ModifiedUtc] = GETUTCDATE()
    WHERE
        [Id] = @Id
ELSE
    INSERT INTO [List].[RecommendedItemsHeaders]
    (
        [CustomerNumber],
        [BranchId],
		[CreatedUtc],
		[ModifiedUtc]
    ) VALUES (
        @CustomerNumber,
        @BranchId,
		GETUTCDATE(),
		GETUTCDATE()
    )
	
SET @ReturnValue = SCOPE_IDENTITY()