DROP INDEX UX_CatalogCampaignHeader_Uri ON [Marketing].[CatalogCampaignHeader]

GO


ALTER TABLE [Marketing].[CatalogCampaignHeader]
	DROP COLUMN [Uri]

GO
