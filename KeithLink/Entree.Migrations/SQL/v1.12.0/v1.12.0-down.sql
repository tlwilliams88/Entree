USE BEK_Commerce_AppData
GO
EXEC sp_rename 'bak__MigrationHistory', '__MigrationHistory';
GO
-- EXEC sp_rename 'bakMigrationHistory', 'MigrationHistory';
GO
DROP TABLE [List].[CustomInventoryItems]

--ALTER TABLE [List].[ListItems] DROP COLUMN Name, Pack, Size, Vendor, CasePrice, PackagePrice, Brand
ALTER TABLE [List].[ListItems] DROP COLUMN CustomInventoryItemId
GO
