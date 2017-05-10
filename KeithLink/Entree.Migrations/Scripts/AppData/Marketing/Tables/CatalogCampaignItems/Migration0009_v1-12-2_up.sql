USE BEK_Commerce_AppData

GO

CREATE TABLE [Marketing].[CatalogCampaignItems] (
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	ItemNumber CHAR(6) NOT NULL,
	CatalogCampaignHeaderId BIGINT NOT NULL,
	Active BIT
)

GO