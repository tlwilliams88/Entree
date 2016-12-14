USE BEK_Commerce_AppData

GO
EXEC sp_rename '__MigrationHistory', 'bak__MigrationHistory';
GO

CREATE TABLE [List].[CustomInventoryItems] (
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ItemNumber VARCHAR(25) NOT NULL,
	CustomerNumber VARCHAR(6) NOT NULL,
	BranchId VARCHAR(3) NOT NULL,
	Name VARCHAR(30) NULL,
	Brand VARCHAR(25) NULL,
	Supplier VARCHAR(30) NULL,
	Pack VARCHAR(4) NULL,
	Size VARCHAR(8) NULL,
	Vendor VARCHAR(6) NULL,
	Each BIT NULL,
	CasePrice DECIMAL(18,2) NULL,
	PackagePrice DECIMAL(18,2) NULL,
	CreatedUtc DATETIME NOT NULL DEFAULT (getutcdate()),
	ModifiedUtc DATETIME NOT NULL DEFAULT (getutcdate())
)

GO
-- Add CustomInventoryItemId column
ALTER TABLE [List].[ListItems] ADD CustomInventoryItemId BIGINT NULL
GO
-- Update ETL.ProcessContractItem stored procedure
-- =============================================
-- Author:		Joshua P. Tirey
-- Create date: 3/29/2015
-- Description:	Creates Entree List for Customer Contract items
-- =============================================
ALTER PROCEDURE [ETL].[ProcessContractItemList] 
AS
BEGIN

SET NOCOUNT ON

DECLARE @customerId varchar(15)
DECLARE @contractNumber varchar(50)
DECLARE @branchID varchar(50)
DECLARE @CurrentBidNumber varchar(15)
DECLARE @CurrentDivision varchar(15)
DECLARE @countT int
DECLARE @existingListId bigint
DECLARE @existingAddedListId bigint
DECLARE @existingDeletedListId bigint

DECLARE @rowCount int
Set @rowCount = 0

SET @CurrentBidNumber = ''
SET @CurrentDivision = ''

DECLARE @TempContractItems TABLE
(
	ItemNumber varchar(10),
	BidNumber varchar(10),
	DivisionNumber varchar(10),
	Each bit,
	BidLineNumber int,
	CategoryDescription varchar(40)
)

DECLARE @AddedItems TABLE
(
	ItemNumber varchar(10),
	Each bit,
	CategoryDescription varchar(100),
	BidLineNumber int
)
DECLARE @DeletedItems TABLE
(
	ItemNumber varchar(10),
	Each bit,
	CategoryDescription varchar(100),
	BidLineNumber int
)

--DECLARE Cursor for all contracts
DECLARE contract_Cursor CURSOR FAST_FORWARD FOR
	SELECT 
		[CustomerNumber]
		,[BidNumber]
		,[DivisionNumber]
	FROM 
		[BEK_Commerce_AppData].[ETL].[Staging_CustomerBid]
	ORDER BY
		BidNumber, DivisionNumber


OPEN contract_Cursor
FETCH NEXT FROM contract_Cursor INTO @customerId, @contractNumber, @branchID

WHILE @@FETCH_STATUS = 0
BEGIN
	Print @rowCount

	IF (@contractNumber <> @CurrentBidNumber OR @branchID <> @CurrentDivision)
	BEGIN		
		DELETE FROM  @TempContractItems
		INSERT INTO @TempContractItems (ItemNumber, BidNumber, DivisionNumber, Each, BidLineNumber, CategoryDescription)
		SELECT 
				LTRIM(RTRIM(d.ItemNumber)),
				LTRIM(RTRIM(d.BidNumber)),
				LTRIM(RTRIM(d.DivisionNumber)),
				CASE WHEN ForceEachOrCaseOnly = 'B' THEN 1 ELSE 0 END AS 'BrokenCaseCode',
				BidLineNumber,
				CategoryDescription
			FROM
				[BEK_Commerce_AppData].[ETL].[Staging_BidContractDetail] d
			WHERE
				BidNumber = @contractNumber AND
				DivisionNumber = @branchID
		
		
		SET @CurrentBidNumber = @contractNumber
		SET @CurrentDivision = @branchID
	END

	
	IF NOT EXISTS(SELECT 'x' FROM @TempContractItems) --No items for this contract, continue to next
	BEGIN
		GOTO cont
	END

	--Find existing contract list for the customer
	SELECT @existingListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 2 AND CustomerId = LTRIM(RTRIM(@customerId)) AND BranchId = LTRIM(RTRIM(@branchID))
	Print @existingListId
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
				('Contract - ' + LTRIM(RTRIM(@contractNumber))
				,2
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
					   ,[Category]
					   ,[Position]
					   ,[Each]
					   ,[CatalogId])
			SELECT 
				LTRIM(RTRIM(ItemNumber)) 'ItemNumber'
				 ,0.00
				,GETUTCDATE()
				, @existingListId
				, GETUTCDATE()
				, CategoryDescription
				, BidLineNumber
				, Each	
				, LTRIM(RTRIM(@branchID))
			FROM 
				@TempContractItems
			ORDER BY
				BidLineNumber ASC

		END 
	ELSE
		BEGIN
			--List already exist. Update with new or deleted items; update contract number.
			UPDATE
				[List].Lists
			SET
				DisplayName = (SELECT CONCAT('Contract - ', LTRIM(RTRIM(@contractNumber)))) 
			WHERE
				[Id] = @existingListId
				
			--Print N'@CurrentBidNumber = ' + @CurrentBidNumber
			--Print N'@existingListId = ' + str(@existingListId)
			--update category on already existing lineitem itemnumbers
			update li
				set li.Category=bd.CategoryDescription
				from List.ListItems as li
				inner join etl.Staging_BidContractDetail bd
					on ltrim(rtrim(bd.BidNumber))=ltrim(rtrim(@CurrentBidNumber)) and LTRIM(rtrim(bd.itemnumber))=li.ItemNumber
				where li.ParentList_Id = @existingListId
	
			--Find new items to be added
			INSERT INTO @AddedItems (ItemNumber, Each, CategoryDescription, BidLineNumber)
			SELECT 
				d.ItemNumber, 
				Each,
				CategoryDescription,
				BidLineNumber
			FROM
				@TempContractItems d
			WHERE 
				NOT EXISTS(SELECT 'x' FROM [BEK_Commerce_AppData].[List].ListItems li 
							WHERE li.ItemNumber = LTRIM(RTRIM(d.ItemNumber)) AND li.Each = d.Each AND li.ParentList_Id = @existingListId)

			--Find items being deleted
			INSERT INTO @DeletedItems (ItemNumber, Each, CategoryDescription, BidLineNumber)
			SELECT
				l.ItemNumber,
				l.Each,
				l.Category,
				l.Position
			FROM
				[BEK_Commerce_AppData].[List].[ListItems] l
			WHERE
				l.ParentList_Id = @existingListId AND
				NOT EXISTS(SELECT 
						'x'
					FROM
						@TempContractItems d
					WHERE
						LTRIM(RTRIM(d.ItemNumber)) = l.ItemNumber AND
						d.Each = l.Each)							

			--New items to add?
			IF EXISTS(SELECT 'x' FROM @AddedItems)
				BEGIN
					--Add to a Contract Added List, create if one doesn't already exist
					
					SELECT @existingAddedListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 6 AND CustomerId = @customerId AND BranchId = @branchID
					
					Print 'Existing added list' 
					Print @existingAddedListId

					IF @existingAddedListId IS NULL
					BEGIN
						Print 'Creating List'
						INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
							([DisplayName]
							,[Type]
							,[CustomerId]
							,[BranchId]
							,[ReadOnly]
							,[CreatedUtc]
							,[ModifiedUtc])
						VALUES
							('Contract Items Added'
							,6
							,@customerId
							,@branchID
							,1
							,GETUTCDATE()
							,GETUTCDATE())

						SET @existingAddedListId = SCOPE_IDENTITY();
					END
					
					--Insert Items into the Contracted Added list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Category]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingAddedListId,
						GETUTCDATE(),
						CategoryDescription,
						BidLineNumber,
						Each,
						@branchID
					FROM
						@AddedItems

					--Insert items into the list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Category]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingListId,
						GETUTCDATE(),
						CategoryDescription,
						BidLineNumber,
						Each,
						@branchID
					FROM
						@AddedItems

				END
				
			--Items to delete
			IF EXISTS(SELECT 'x' FROM @DeletedItems)
				BEGIN
					--Add to a Contract Added List, create if one doesn't already exist
					
					SELECT @existingDeletedListId = Id from [BEK_Commerce_AppData].[List].Lists WHERE Type = 7 AND CustomerId = @customerId AND BranchId = @branchID
										
					IF @existingDeletedListId IS NULL
					BEGIN
						INSERT INTO [BEK_Commerce_AppData].[List].[Lists]
							([DisplayName]
							,[Type]
							,[CustomerId]
							,[BranchId]
							,[ReadOnly]
							,[CreatedUtc]
							,[ModifiedUtc])
						VALUES
							('Contract Items Deleted'
							,7
							,@customerId
							,@branchID
							,1
							,GETUTCDATE()
							,GETUTCDATE())

						SET @existingDeletedListId = SCOPE_IDENTITY();
					END


					--DELETE Item
					DELETE [BEK_Commerce_AppData].[List].[ListItems]
					FROM [BEK_Commerce_AppData].[List].[ListItems] li inner join
						@DeletedItems d on li.ItemNumber = d.ItemNumber AND li.Each = d.Each and ParentList_Id = @existingListId
					
					--Insert Items into the Contracted Deleted list
					INSERT INTO [BEK_Commerce_AppData].[List].[ListItems]
							   ([ItemNumber]
							   ,[Par]
							   ,[CreatedUtc]
							   ,[ParentList_Id]
							   ,[ModifiedUtc]
							   ,[Category]
							   ,[Position]
							   ,[Each]
							   ,[CatalogId])
					SELECT
						LTRIM(RTRIM(ItemNumber)),
						0.00,
						GETUTCDATE(),
						@existingDeletedListId,
						GETUTCDATE(),
						CategoryDescription,
						BidLineNumber,
						Each,
						@branchID
					FROM
						@DeletedItems
				END				
			

		END

cont:
	
	SET @existingListId = null
	SET @existingAddedListId = null
	SET @existingDeletedListId = null
	DELETE FROM @AddedItems
	DELETE FROM @DeletedItems
	SET @rowCount = @rowCount +1
	FETCH NEXT FROM contract_Cursor INTO @customerId, @contractNumber, @branchID
END

close contract_Cursor
DEALLOCATE contract_Cursor
END