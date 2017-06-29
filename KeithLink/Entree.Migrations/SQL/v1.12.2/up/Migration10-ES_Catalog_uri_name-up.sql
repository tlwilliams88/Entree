

GO

ALTER TABLE [Marketing].[CatalogCampaignHeader]
	ADD [Uri] VARCHAR(255);

GO

CREATE UNIQUE INDEX UX_CatalogCampaignHeader_Uri
	ON [Marketing].[CatalogCampaignHeader] (Uri);

GO

/* Create new procedures */

CREATE PROCEDURE [Marketing].[GetCatalogCampaignHeaderByUri] 
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

/* Edit existing procedures */

ALTER PROCEDURE [Marketing].[GetAllCatalogCampaignHeader] 
AS
	SELECT
		[Id],
		[Uri],
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
		[Uri],
		[Description],
		[Active],
		[StartDate],
		[EndDate]
	FROM [Marketing].[CatalogCampaignHeader] 
	WHERE [Id] = @id
GO


