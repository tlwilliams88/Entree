EXEC sp_RENAME 'Marketing.CatalogCampaignHeader.Description' , 'Name' , 'COLUMN'
GO
ALTER TABLE Marketing.CatalogCampaignHeader ADD [Description] VARCHAR (5000);
GO
UPDATE Marketing.CatalogCampaignHeader SET [Description]=[Name]
GO
