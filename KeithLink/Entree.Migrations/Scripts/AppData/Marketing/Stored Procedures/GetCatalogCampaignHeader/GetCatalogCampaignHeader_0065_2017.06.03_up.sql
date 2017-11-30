
ALTER PROCEDURE [Marketing].[GetCatalogCampaignHeader] 
	@id bigint
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
	WHERE [Id] = @id
GO

