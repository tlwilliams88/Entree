/****** Object:  StoredProcedure [ETL].[ProcessWorksheetList]    Script Date: 10/27/2016 1:05:24 PM ******/
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO

-- =============================================
-- Author:      Joshua P. Tirey
-- Create date: 3/29/2015
-- Description: Creates Entree list for Customer History (worksheet) items
-- =============================================
ALTER PROCEDURE [ETL].[ProcessWorksheetList]
AS
BEGIN

DECLARE @customerId varchar(15)
DECLARE @branchID varchar(50)


DECLARE @CurrentBidNumber varchar(15)
DECLARE @CurrentDivision varchar(15)
DECLARE @countT int
DECLARE @existingListId bigint
DECLARE @existingHistoryHeaderId bigint

DECLARE @AddedItems TABLE
(
    ItemNumber varchar(10),
    Each bit
)
DECLARE @DeletedItems TABLE
(
    ItemNumber varchar(10),
    Each bit
)

--DECLARE Cursor for all contracts
DECLARE worksheet_Cursor CURSOR FAST_FORWARD FOR
    SELECT 
        [CustomerNumber],
        [DivisionNumber]
    FROM 
        [BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems]
    GROUP BY
        [CustomerNumber],
        [DivisionNumber]


OPEN worksheet_Cursor
FETCH NEXT FROM worksheet_Cursor INTO @customerId, @branchID

WHILE @@FETCH_STATUS = 0
BEGIN
    --Find existing worksheet list for the customer
    SELECT @existingHistoryHeaderId = [Id] 
        from [BEK_Commerce_AppData].[List].[HistoryHeaders] 
        WHERE   [CustomerNumber] = LTRIM(RTRIM(@customerId)) 
                AND [BranchId] = LTRIM(RTRIM(@branchID))
    
    IF @existingHistoryHeaderId IS NULL
        BEGIN
            --List doesn't exist -- Create list
            INSERT INTO [BEK_Commerce_AppData].[List].[HistoryHeaders]
                ([CustomerNumber]
                ,[BranchId]
                ,[CreatedUtc]
                ,[ModifiedUtc])
            VALUES
                (
                 LTRIM(RTRIM(@customerId))
                ,LTRIM(RTRIM(@branchID))
                ,GETUTCDATE()
                ,GETUTCDATE())

            SET @existingHistoryHeaderId = SCOPE_IDENTITY();

            --Insert items into the new list
            INSERT INTO [BEK_Commerce_AppData].[List].[HistoryDetails]
                       ([ItemNumber]
                       ,[CreatedUtc]
                       ,[HeaderId]
                       ,[ModifiedUtc]
                       ,[LineNumber]
                       ,[Each]
                       ,[CatalogId])
            SELECT 
                LTRIM(RTRIM(ItemNumber)),
                GETUTCDATE(),
                @existingHistoryHeaderId,
                GETUTCDATE(),
                ROW_NUMBER() over (Order By ItemNumber),
                CASE WHEN BrokenCaseCode = 'Y' THEN 1 ELSE 0 END,
                @branchID
            FROM 
                [BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems]
            WHERE
                [CustomerNumber] = @customerId 
                AND [DivisionNumber] = @branchID

        END
    ELSE
        BEGIN
            --List already exist. Update with new or deleted items          
            
            --Find new items to be added
            INSERT INTO @AddedItems (ItemNumber, Each)
            SELECT 
                LTRIM(RTRIM(w.ItemNumber)), 
                CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END
            FROM
                [BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems] w
            WHERE
                w.CustomerNumber = @customerId AND
                w.DivisionNumber = @branchID AND
                NOT EXISTS(SELECT 'x' FROM [BEK_Commerce_AppData].[List].[HistoryDetails] li 
                                WHERE li.ItemNumber = LTRIM(RTRIM(w.ItemNumber)) 
                                    AND li.Each = CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END 
                                    AND li.[HeaderId] = @existingHistoryHeaderId)                      

            --Find items being deleted
            INSERT INTO @DeletedItems (ItemNumber, Each)
            SELECT
                l.ItemNumber,
                l.Each
            FROM
                [BEK_Commerce_AppData].[List].[HistoryDetails] l
            WHERE
                l.[HeaderId] = @existingHistoryHeaderId AND
                NOT EXISTS(SELECT 
                        'x'
                    FROM
                        [BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems] w
                    WHERE
                        LTRIM(RTRIM(w.ItemNumber)) = l.ItemNumber AND
                        CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END = l.Each AND
                        w.CustomerNumber = @customerId AND w.DivisionNumber = @branchID)

            --New items to add?
            IF EXISTS(SELECT 'x' FROM @AddedItems)
                BEGIN
                    --Insert items into the list
                INSERT INTO [BEK_Commerce_AppData].[List].[HistoryDetails]
                           ([ItemNumber]
                           ,[CreatedUtc]
                           ,[HeaderId]
                           ,[ModifiedUtc]
                           ,[LineNumber]
                           ,[Each]
                           ,[CatalogId])
                    SELECT
                        LTRIM(RTRIM(ItemNumber)),
                        GETUTCDATE(),
                        @existingHistoryHeaderId,
                        GETUTCDATE(),
                        0,
                        Each,
                        @branchID
                    FROM
                        @AddedItems

                END

            --Items to delete
            IF EXISTS(SELECT 'x' FROM @DeletedItems)
                BEGIN
                    --DELETE Item
                    DELETE [BEK_Commerce_AppData].[List].[HistoryDetails]
                    FROM [BEK_Commerce_AppData].[List].[HistoryDetails] li INNER JOIN
                        @DeletedItems d on li.ItemNumber = d.ItemNumber AND li.Each = d.Each AND [HeaderId] = @existingHistoryHeaderId
                                        
                END     
            

            --update all list position numbers 
            -- also update the catalog id to make sure that they get set
            UPDATE [BEK_Commerce_AppData].[List].[HistoryDetails] 
            SET [LineNumber] = p.Positions
                , CatalogId = @branchID
            FROM [BEK_Commerce_AppData].[List].[HistoryDetails]
                INNER JOIN
                    (
                        SELECT
                            ItemNumber 'p_ItemNumber', 
                            [HeaderId] 'p_ListId',
                            RANK() OVER (ORDER BY ItemNumber) Positions
                        FROM
                             [BEK_Commerce_AppData].[List].[HistoryDetails]
                        WHERE
                             [HeaderId] = @existingHistoryHeaderId
                    ) as p
                ON ItemNumber = p.p_ItemNumber
                AND [HeaderId] = p.p_ListId
        END

    SET @existingHistoryHeaderId = null
    DELETE FROM @AddedItems
    DELETE FROM @DeletedItems
    
    FETCH NEXT FROM worksheet_Cursor INTO @customerId, @branchID
END

CLOSE worksheet_Cursor
DEALLOCATE worksheet_Cursor
END

GO
