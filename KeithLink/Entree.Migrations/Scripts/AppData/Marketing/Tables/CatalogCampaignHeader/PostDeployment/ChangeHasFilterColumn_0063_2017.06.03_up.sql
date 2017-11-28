UPDATE
    Marketing.CatalogCampaignHeader
SET
    HasFilter = 0
GO

ALTER TABLE Marketing.CatalogCampaignHeader
    ALTER COLUMN
        HasFilter   BIT NOT NULL
GO