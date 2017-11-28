ALTER PROCEDURE [Marketing].[GetAllCatalogCampaignHeader] 
AS
	SELECT
		[Id],
		[Uri],
		[Name],
		[Description],
		[Active],
		[StartDate],
		[EndDate],
        [HasFilter]
	FROM [Marketing].[CatalogCampaignHeader] 
    WHERE Active = 1
      AND StartDate <= GETDATE()
      AND EndDate > GETDATE()
GO
