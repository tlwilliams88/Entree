USE BEK_Commerce_AppData

GO
-- Clean up bad data created from previous version
DELETE FROM [List].[ListItems]
	WHERE Id IN (SELECT l.Id FROM [List].[ListItems] l
			LEFT JOIN [List].[CustomInventoryItems] c ON c.Id = l.CustomInventoryItemId
			WHERE l.CustomInventoryItemId IS NOT NULL
			AND   c.Id IS NULL)

GO

-- Add label column to custom inventory items
ALTER TABLE [List].[CustomInventoryItems] ADD Label nvarchar(150)  NULL

GO

-- Custom type for passing an array of bigints into stored procedures
CREATE TYPE BigIntList AS TABLE (Id bigint NOT NULL PRIMARY KEY)

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