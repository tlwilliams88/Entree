ALTER TABLE Marketing.CatalogCampaignHeader DROP COLUMN [Description];
GO
EXEC sp_RENAME 'Marketing.CatalogCampaignHeader.Name' , 'Description' , 'COLUMN'
GO