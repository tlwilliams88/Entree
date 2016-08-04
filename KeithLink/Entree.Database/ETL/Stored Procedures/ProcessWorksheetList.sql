
-- =============================================
-- Author:		Joshua P. Tirey
-- Create date: 3/29/2015
-- Description:	Creates Entree list for Customer History (worksheet) items
-- =============================================
CREATE PROCEDURE [ETL].[ProcessWorksheetList]
AS
BEGIN

DECLARE @customerId varchar(15)
DECLARE @branchID varchar(50)


DECLARE @CurrentBidNumber varchar(15)
DECLARE @CurrentDivision varchar(15)
DECLARE @countT int
DECLARE @existingListId bigint

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
DECLARE worksheet_Cursor CURSOR FOR
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
	SELECT @existingListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 5 AND CustomerId = LTRIM(RTRIM(@customerId)) AND BranchId = LTRIM(RTRIM(@branchID))
	
	IF @existingListId IS NULL
		BEGIN
			--List doesn't exist -- Create list
			INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
				([DisplayName]
				,[Type]
				,[CustomerId]
				,[BranchId]
				,[ReadOnly]
				,[CreatedUtc]
				,[ModifiedUtc])
			VALUES
				('History'
				,5
				,LTRIM(RTRIM(@customerId))
				,LTRIM(RTRIM(@branchID))
				,1
				,GETUTCDATE()
				,GETUTCDATE())

			SET @existingListId = SCOPE_IDENTITY();

			--Insert items into the new list
			INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
					   ([ItemNumber]
					   ,[Par]
					   ,[CreatedUtc]
					   ,[ParentList_Id]
					   ,[ModifiedUtc]
					   ,[Position]
					   ,[Each]
					   ,[CatalogId])
			SELECT 
				LTRIM(RTRIM(ItemNumber)),
				0.00,
				GETUTCDATE(),
				@existingListId,
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
				NOT EXISTS(SELECT top 1 * FROM [BEK_Commerce_AppData].[List].ListItems li 
							WHERE li.ItemNumber = LTRIM(RTRIM(w.ItemNumber)) AND li.Each = CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END AND li.ParentList_Id = @existingListId)
						

			--Find items being deleted
			INSERT INTO @DeletedItems (ItemNumber, Each)
			SELECT
				l.ItemNumber,
				l.Each
			FROM
				[BEK_Commerce_AppData].[List].[ListItems] l
			WHERE
				l.ParentList_Id = @existingListId AND
				NOT EXISTS(SELECT 
						top 1 *
					FROM
						[BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems] w
					WHERE
						LTRIM(RTRIM(w.ItemNumber)) = l.ItemNumber AND
						CASE WHEN w.BrokenCaseCode = 'Y' THEN 1 ELSE 0 END = l.Each AND
						w.CustomerNumber = @customerId AND w.DivisionNumber = @branchID)
			--New items to add?
			IF EXISTS(SELECT top 1 * FROM @AddedItems)
				BEGIN
					--Insert items into the list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingListId,
						GETUTCDATE(),
						0,
						Each,
						@branchID
					FROM
						@AddedItems

				END

			--Items to delete
			IF EXISTS(SELECT top 1 * FROM @DeletedItems)
				BEGIN
					--DELETE Item
					DELETE [BEK_Commerce_AppData].[List].[ListItems]
					FROM [BEK_Commerce_AppData].[List].[ListItems] li INNER JOIN
						@DeletedItems d on li.ItemNumber = d.ItemNumber AND li.Each = d.Each AND ParentList_Id = @existingListId
										
				END		
			

			--update all list position numbers 
			-- also update the catalog id to make sure that they get set
			UPDATE List.ListItems 
			SET Position = p.Positions
				, CatalogId = @branchID
			FROM List.Listitems 
				INNER JOIN
					(
						SELECT
							ItemNumber 'p_ItemNumber', 
							ParentList_Id 'p_ListId',
							RANK() OVER (ORDER BY ItemNumber) Positions
						FROM
							 List.listItems
						WHERE
							 ParentList_Id = @existingListId
					) as p
				ON ItemNumber = p.p_ItemNumber
				AND ParentList_Id = p.p_ListId
		END

	SET @existingListId = null
	DELETE FROM @AddedItems
	DELETE FROM @DeletedItems
	
	FETCH NEXT FROM worksheet_Cursor INTO @customerId, @branchID
END

CLOSE worksheet_Cursor
DEALLOCATE worksheet_Cursor
END