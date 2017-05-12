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

GO-- Add label column to custom inventory items
ALTER TABLE [List].[CustomInventoryItems] ADD Label nvarchar(150)  NULL