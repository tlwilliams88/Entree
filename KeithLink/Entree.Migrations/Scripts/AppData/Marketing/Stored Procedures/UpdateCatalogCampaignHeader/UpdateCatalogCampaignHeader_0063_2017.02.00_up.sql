﻿ALTER PROCEDURE [Marketing].[UpdateCatalogCampaignHeader]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@Name          	VARCHAR (250),
			@Description	VARCHAR (5000),
			@StartDate		DATETIME,
			@EndDate		DATETIME,
			@Active			BIT,
			@Uri			VARCHAR (250),
            @HasFilter      BIT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [Marketing].[CatalogCampaignHeader]
	SET [Name] = @Name
	    , [Description] = @Description
		, [Active] = @Active
		, [StartDate] = @StartDate
		, [EndDate] = @EndDate
        , [HasFilter] = @HasFilter
	WHERE [Uri] = @Uri

    SELECT [Id]
	FROM [Marketing].[CatalogCampaignHeader]
	WHERE [Uri] = @Uri

END