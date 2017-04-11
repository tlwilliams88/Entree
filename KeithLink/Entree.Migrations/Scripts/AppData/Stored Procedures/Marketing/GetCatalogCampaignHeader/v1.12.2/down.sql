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