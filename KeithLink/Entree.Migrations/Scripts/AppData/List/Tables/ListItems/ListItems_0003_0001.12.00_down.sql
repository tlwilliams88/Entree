-- Clean up the data first
DELETE FROM [List].[ListItems] WHERE [CustomInventoryItemId] IS NOT NULL
GO
--ALTER TABLE [List].[ListItems] DROP COLUMN Name, Pack, Size, Vendor, CasePrice, PackagePrice, Brand
ALTER TABLE [List].[ListItems] DROP COLUMN CustomInventoryItemId
GO