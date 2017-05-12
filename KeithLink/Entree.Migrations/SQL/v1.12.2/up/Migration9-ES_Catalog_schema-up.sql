

GO

CREATE SCHEMA [Marketing];

GO

CREATE TABLE [Marketing].[CatalogCampaignHeader] (
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Description] VARCHAR(250) NOT NULL,
	Active BIT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL
)

GO

CREATE TABLE [Marketing].[CatalogCampaignItems] (
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ItemNumber CHAR(6) NOT NULL,
	CatalogCampaignHeaderId BIGINT NOT NULL,
	Active BIT
)

GO

/*
	Catalog Campaign Header
*/
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

CREATE PROCEDURE [Marketing].[GetCatalogCampaignHeader] 
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


/*
	Catalog Campaign Items Procedures
*/

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
