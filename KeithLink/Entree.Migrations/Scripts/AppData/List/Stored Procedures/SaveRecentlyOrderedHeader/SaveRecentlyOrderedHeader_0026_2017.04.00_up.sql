CREATE PROCEDURE [List].[SaveRecentlyOrderedHeader] 
    @Id                             BIGINT,
    @UserId                         UNIQUEIDENTIFIER,
	@CustomerNumber					CHAR(6),
    @BranchId                       CHAR(3),
    @ReturnValue                    BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[RecentlyOrderedHeaders]
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
        INSERT INTO [List].[RecentlyOrderedHeaders]
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