﻿CREATE PROC [List].[SaveFavoritesHeader]
    @UserId         UNIQUEIDENTIFIER,
    @BranchId       CHAR(3),
    @CustomerNumber CHAR(6)
AS
    IF NOT EXISTS(SELECT 
                    'X'
                  FROM
                    [List].[FavoritesHeaders]
                  WHERE
                    [UserId] = @UserId
                  AND
                    [BranchId] = @BranchId
                  AND
                    [CustomerNumber] = @CustomerNumber)
        INSERT INTO 
            [List].[FavoritesHeaders] (
                [UserId],
                [BranchId],
                [CustomerNumber],
                [CreatedUtc],
                [ModifiedUtc]
            ) VALUES (
                @UserId,
                @BranchId,
                @CustomerNumber,
                GETUTCDATE(),
                GETUTCDATE()
            )

        RETURN @@IDENTITY
GO
    