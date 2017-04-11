USE BEK_Commerce_AppData
GO
ALTER TABLE [List].[CustomInventoryItems] DROP COLUMN Label
GO
DROP PROCEDURE [List].[DeleteCustomInventoryItems]
GO
DROP TYPE BigIntList
GO
