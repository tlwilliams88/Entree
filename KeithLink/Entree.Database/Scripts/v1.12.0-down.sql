USE BEK_Commerce_AppData
GO
EXEC sp_rename 'bak__MigrationHistory', '__MigrationHistory';
GO
EXEC sp_rename 'bakMigrationHistory', 'MigrationHistory';
GO
ALTER TABLE [List].[ListItems] DROP COLUMN Name, Pack, Size, Vendor, CasePrice, PackagePrice
GO
