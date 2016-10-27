USE BEK_Commerce_AppData
GO
EXEC sp_rename '__MigrationHistory', 'bak__MigrationHistory';
GO
EXEC sp_rename 'MigrationHistory', 'bakMigrationHistory';
GO
ALTER TABLE [List].[ListItems] ADD Name VARCHAR(30) NULL, Pack VARCHAR(4) NULL, Size VARCHAR(8) NULL, Vendor VARCHAR(6) NULL
GO
ALTER TABLE [List].[ListItems] ADD CasePrice VARCHAR(10) NULL, PackagePrice VARCHAR(10) NULL 
GO