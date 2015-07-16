USE [Bek_Commerce_Appdata]
GO
/****** Object:  StoredProcedure [ETL].[ProcessWorksheetList]    Script Date: 7/16/2015 11:10:06 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Batch submitted through debugger: ETL_Process_Lists_and_ListItems.sql|7|0|C:\Users\jcswaringen\Documents\SQL Server Management Studio\ETL_Process_Lists_and_ListItems.sql

-- =============================================
-- Author:		Joshua P. Tirey
-- Create date: 3/29/2015
-- Description:	Creates Entree list for Customer History (worksheet) items
-- =============================================
ALTER PROCEDURE [ETL].[ProcessWorksheetList]
AS
BEGIN

	-- Clear Lists and ListItem tables.
	DECLARE @ClearTables VARCHAR(20);
	SELECT @ClearTables = 'ClearTables';

	BEGIN TRANSACTION @ClearTables;
		-- Remove foreign key constraint from list items table.
		ALTER TABLE [List].[ListItems] DROP CONSTRAINT [FK_List.ListItems_List.Lists_List_Id];
		

		-- Delete list items & reset index.
		DELETE FROM [List].ListItems

		
		-- Delete lists and reset index.
		DELETE FROM [List].Lists

		-- Add foreign key constraint back to ListItems table.
		ALTER TABLE [List].[ListItems]  WITH NOCHECK 
			ADD CONSTRAINT [FK_List.ListItems_List.Lists_List_Id] FOREIGN KEY([ParentList_Id])
				REFERENCES [List].[Lists] ([Id])

	If @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION @ClearTables
			PRINT N'Error = ' + CAST(@@ERROR AS NVARCHAR(8));
		END
	ELSE
		BEGIN
			COMMIT TRANSACTION @ClearTables
		END
	END
	
	BEGIN

		-- Variables
		DECLARE @customerId varchar(15)
		DECLARE @branchID varchar(50)
		DECLARE @existingListId bigint



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


			-- Create list
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
						,[Each])
			SELECT 
				LTRIM(RTRIM(ItemNumber)),
				0.00,
				GETUTCDATE(),
				@existingListId,
				GETUTCDATE(),
				ROW_NUMBER() over (Order By ItemNumber),
				CASE WHEN BrokenCaseCode = 'Y' THEN 1 ELSE 0 END
			FROM 
				[BEK_Commerce_AppData].[ETL].[Staging_WorksheetItems]
			WHERE
				[CustomerNumber] = @customerId AND
				[DivisionNumber] = @branchID

			SET @existingListId = null
	
			FETCH NEXT FROM worksheet_Cursor INTO @customerId, @branchID
		END

	close worksheet_Cursor
	DEALLOCATE worksheet_Cursor
	END
END