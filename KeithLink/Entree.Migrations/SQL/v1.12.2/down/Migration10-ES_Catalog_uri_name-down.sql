USE BEK_Commerce_AppData

GO

DROP INDEX UX_CatalogCampaignHeader_Uri ON [Marketing].[CatalogCampaignHeader]

GO

DROP PROCEDURE [Marketing].[GetCatalogCampaignHeaderbyUri]

GO

ALTER TABLE [Marketing].[CatalogCampaignHeader]
	DROP COLUMN [Uri]

GO

ALTER PROCEDURE [Marketing].[GetAllCatalogCampaignHeader] 
AS
	SELECT
		[Id],
		[Description],
		[Active],
		[StartDate],
		[EndDate]
	FROM [Marketing].[CatalogCampaignHeader] 
GO

ALTER PROCEDURE [Marketing].[GetCatalogCampaignHeader] 
	@id bigint
AS
	SELECT
		[Id],
		[Description],
		[Active],
		[StartDate],
		[EndDate]
	FROM [Marketing].[CatalogCampaignHeader] 
	WHERE [Id] = @id
GO
