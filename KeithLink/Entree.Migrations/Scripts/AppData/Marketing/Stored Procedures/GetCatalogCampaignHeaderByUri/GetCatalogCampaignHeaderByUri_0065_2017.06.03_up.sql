ALTER PROCEDURE [Marketing].[GetCatalogCampaignHeaderByUri] 
	@Uri VARCHAR(255)
AS
	SELECT
		[Id],
        [Name],
		[Uri],
		[Description],
		[Active],
		[StartDate],
		[EndDate],
        [HasFilter],
        [LinkToUrl]
	FROM [Marketing].[CatalogCampaignHeader] 
	WHERE [Uri] = @Uri
GO
