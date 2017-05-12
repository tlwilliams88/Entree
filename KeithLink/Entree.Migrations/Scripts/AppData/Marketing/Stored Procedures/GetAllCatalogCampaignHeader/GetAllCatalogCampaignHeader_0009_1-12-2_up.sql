

GO

CREATE PROCEDURE [Marketing].[GetAllCatalogCampaignHeader] 
AS
	SELECT
		[Id],
		[Description],
		[Active],
		[StartDate],
		[EndDate]
	FROM [Marketing].[CatalogCampaignHeader] 
GO