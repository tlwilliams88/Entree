

GO

CREATE PROCEDURE [Marketing].[GetAllCatalogCampaignItemsByHeader] 
	@catalogCampaignHeaderId bigint
AS
	SELECT
		[Id],
		[ItemNumber],
		[CatalogCampaignHeaderId],
		[Active]
	FROM [Marketing].[CatalogCampaignItems] 
	WHERE [CatalogCampaignHeaderId] = @catalogCampaignHeaderId
GO