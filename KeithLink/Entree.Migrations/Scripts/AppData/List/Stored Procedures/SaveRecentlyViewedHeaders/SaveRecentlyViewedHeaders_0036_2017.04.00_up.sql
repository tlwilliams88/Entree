CREATE PROCEDURE [List].[SaveRecentlyViewedHeader] 
    @Id                             BIGINT,
    @UserId                         UNIQUEIDENTIFIER,
	@CustomerNumber					CHAR(6),
    @BranchId                       CHAR(3),
    @ReturnValue                    BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[RecentlyViewedHeaders]
        SET
            [UserId] = @UserId,
            [BranchId] = @BranchId,
            [CustomerNumber] = @CustomerNumber,
			[ModifiedUtc] = GETUTCDATE()
        WHERE
            [Id] = @Id
    END
ELSE
    BEGIN
        INSERT INTO [List].[RecentlyViewedHeaders]
        (
            [UserId],
            [BranchId],
            [CustomerNumber],
			[CreatedUtc],
			[ModifiedUtc]
        )
        VALUES
        (
            @UserId,
            @BranchId,
            @CustomerNumber,
			GETUTCDATE(),
			GETUTCDATE()
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()