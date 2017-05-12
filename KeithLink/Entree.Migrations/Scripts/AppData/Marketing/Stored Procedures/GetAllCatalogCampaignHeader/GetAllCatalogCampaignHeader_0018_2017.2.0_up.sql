ALTER PROCEDURE [Marketing].[GetAllCatalogCampaignHeader] 
AS
	SELECT
		[Id],
		[Uri],
		[Name],
		[Description],
		[Active],
		[StartDate],
		[EndDate]
	FROM [Marketing].[CatalogCampaignHeader] 
GO
