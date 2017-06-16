CREATE PROCEDURE [List].[SaveRecentlyOrderedHeader] 
    @Id                             BIGINT,
    @UserId                         UNIQUEIDENTIFIER,
    @BranchId                       CHAR(3),
    @ItemNumber                     VARCHAR(6),
    @ReturnValue                    BIGINT OUTPUT
AS

IF @Id > 0
    BEGIN
        UPDATE [List].[RecentlyOrderedHeaders]
        SET
            [UserId] = @UserId,
            [BranchId] = @BranchId,
            [CustomerNumber] = @CustomerNumber
        WHERE
            [Id] = @Id
    END
ELSE
    BEGIN
        INSERT INTO [List].[RecentlyOrderedHeaders]
        (
            [UserId],
            [BranchId],
            [CustomerNumber]
        )
        VALUES
        (
            @UserId,
            @BranchId,
            @CustomerNumber
        )
    END

SET @ReturnValue = SCOPE_IDENTITY()