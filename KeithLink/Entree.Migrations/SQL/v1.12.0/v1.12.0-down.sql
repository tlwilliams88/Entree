USE BEK_Commerce_AppData
GO
-- Get rid of EF migrations table
EXEC sp_rename 'bak__MigrationHistory', '__MigrationHistory';
GO
DROP TABLE [List].[CustomInventoryItems]
GO
-- Clean up the data first
DELETE FROM [List].[ListItems] WHERE [CustomInventoryItemId] IS NOT NULL
GO
--ALTER TABLE [List].[ListItems] DROP COLUMN Name, Pack, Size, Vendor, CasePrice, PackagePrice, Brand
ALTER TABLE [List].[ListItems] DROP COLUMN CustomInventoryItemId
GO
