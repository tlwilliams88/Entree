USE BEK_Commerce_AppData

GO
EXEC sp_rename '__MigrationHistory', 'bak__MigrationHistory';
GO
-- EXEC sp_rename 'MigrationHistory', 'bakMigrationHistory';
GO

CREATE TABLE [List].[CustomInventoryItems] (
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ItemNumber CHAR(25) NOT NULL,
	CustomerNumber CHAR(6) NOT NULL,
	BranchId CHAR(3) NOT NULL,
	Name VARCHAR(30) NULL,
	Brand VARCHAR(25) NULL,
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
/* ALTER TABLE [List].[ListItems] ADD 
Name VARCHAR(30) NULL, 
Pack VARCHAR(4) NULL, 
Size VARCHAR(8) NULL, 
Vendor VARCHAR(6) NULL

*/

-- Add CustomInventoryItemId column
ALTER TABLE [List].[ListItems] ADD CustomInventoryItemId INT NULL
GO
--ALTER TABLE [List].[ListItems] ADD CasePrice VARCHAR(10) NULL, PackagePrice VARCHAR(10) NULL, Brand VARCHAR(6) NULL
GO