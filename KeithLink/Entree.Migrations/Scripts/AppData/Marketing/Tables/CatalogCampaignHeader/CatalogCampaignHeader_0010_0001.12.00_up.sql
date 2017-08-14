ALTER TABLE [Marketing].[CatalogCampaignHeader]
	ADD [Uri] VARCHAR(255);

GO

CREATE UNIQUE INDEX UX_CatalogCampaignHeader_Uri
	ON [Marketing].[CatalogCampaignHeader] (Uri);

GO
