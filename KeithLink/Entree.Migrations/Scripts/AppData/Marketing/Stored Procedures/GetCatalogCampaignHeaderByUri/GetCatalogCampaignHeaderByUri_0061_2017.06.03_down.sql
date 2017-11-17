ALTER PROCEDURE [Marketing].[GetCatalogCampaignHeaderByUri] 
	@Uri VARCHAR(255)
AS
	SELECT
		[Id],
		[Uri],
		[Description],
		[Active],
		[StartDate],
		[EndDate]
	FROM [Marketing].[CatalogCampaignHeader] 
	WHERE [Uri] = @Uri
GO
