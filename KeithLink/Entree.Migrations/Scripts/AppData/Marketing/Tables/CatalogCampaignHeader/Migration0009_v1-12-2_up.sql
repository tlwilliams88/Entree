USE BEK_Commerce_AppData

GO

CREATE TABLE [Marketing].[CatalogCampaignHeader] (
	Id BIGINT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	[Description] VARCHAR(250) NOT NULL,
	Active BIT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL
)

GO