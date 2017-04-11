USE BEK_Commerce_AppData

GO

-- Stored Procedure for batch deleting custom inventory items
CREATE PROCEDURE [List].[DeleteCustomInventoryItems]
	@Ids BigIntList READONLY 
AS
	-- Remove all custom inventory items with matching keys from the list items table first
	DELETE FROM [List].[ListItems]
	WHERE CustomInventoryItemId IN (SELECT Id FROM @Ids)

	-- Remove all custom inventory items from the custom inventory item table
	DELETE FROM [List].[CustomInventoryItems]
	WHERE Id IN (SELECT Id FROM @Ids)
GO