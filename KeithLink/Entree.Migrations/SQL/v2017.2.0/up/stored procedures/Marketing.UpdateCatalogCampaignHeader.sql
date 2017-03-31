CREATE PROCEDURE [Marketing].[UpdateCatalogCampaignHeader]
-- =============================================
-- Author:		Brett Killins
-- Create date: 2/28/2017
-- Description:	<Description,,>
-- =============================================
			@Description	VARCHAR (250),
			@StartDate		DATETIME,
			@EndDate		DATETIME,
			@Active			BIT,
			@Uri			VARCHAR (250)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE [Marketing].[CatalogCampaignHeader]
	SET [Description] = @Description
		, [Active] = @Active
		, [StartDate] = @StartDate
		, [EndDate] = @EndDate
	WHERE [Uri] = @Uri

    SELECT [Id]
	FROM [Marketing].[CatalogCampaignHeader]
	WHERE [Uri] = @Uri

END